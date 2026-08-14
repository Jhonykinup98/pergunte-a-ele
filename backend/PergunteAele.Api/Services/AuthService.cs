using Microsoft.EntityFrameworkCore;
using PergunteAele.Api.Data;
using PergunteAele.Api.Models;
using PergunteAele.Api.Models.DTOs;

namespace PergunteAele.Api.Services;

public interface IAuthService
{
    Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request);
    Task<(bool Success, string? Error, AuthResponse? Response)> LoginAsync(LoginRequest request);
    Task<(bool Success, string? Error)> ConfirmEmailAsync(string token);
    Task<(bool Success, string? Error, AuthResponse? Response)> GoogleLoginAsync(string idToken);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly GoogleAuthService _googleAuthService;

    public AuthService(
        AppDbContext db,
        ITokenService tokenService,
        IEmailService emailService,
        GoogleAuthService googleAuthService)
    {
        _db = db;
        _tokenService = tokenService;
        _emailService = emailService;
        _googleAuthService = googleAuthService;
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(RegisterRequest request)
    {
        if (request.Password != request.ConfirmPassword)
            return (false, "As senhas não coincidem.");

        if (request.Password.Length < 8)
            return (false, "A senha precisa ter pelo menos 8 caracteres.");

        var alreadyExists = await _db.Users.AnyAsync(u =>
            u.Email == request.Email || u.Login == request.Login);
        if (alreadyExists)
            return (false, "Já existe uma conta com este login ou e-mail.");

        var user = new User
        {
            Login = request.Login,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Provider = "local",
            EmailConfirmed = false,
            EmailConfirmationToken = Guid.NewGuid().ToString("N")
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        await _emailService.SendConfirmationEmailAsync(user.Email, user.EmailConfirmationToken!);

        return (true, null);
    }

    public async Task<(bool Success, string? Error, AuthResponse? Response)> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == request.Login);

        if (user is null || user.PasswordHash is null)
            return (false, "Login ou senha inválidos.", null);

        var passwordOk = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!passwordOk)
            return (false, "Login ou senha inválidos.", null);

        if (!user.EmailConfirmed)
            return (false, "Confirme seu e-mail antes de entrar.", null);

        var token = _tokenService.GenerateToken(user);
        return (true, null, new AuthResponse(token, user.Login, user.Email, user.EmailConfirmed, user.Role));
    }

    public async Task<(bool Success, string? Error)> ConfirmEmailAsync(string token)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.EmailConfirmationToken == token);
        if (user is null)
            return (false, "Token de confirmação inválido.");

        user.EmailConfirmed = true;
        user.EmailConfirmationToken = null;
        await _db.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error, AuthResponse? Response)> GoogleLoginAsync(string idToken)
    {
        var payload = await _googleAuthService.ValidateAsync(idToken);
        if (payload is null)
            return (false, "Token do Google inválido.", null);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

        if (user is null)
        {

            user = new User
            {
                Login = payload.Email.Split('@')[0],
                Email = payload.Email,
                PasswordHash = null,
                Provider = "google",
                EmailConfirmed = true
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        var token = _tokenService.GenerateToken(user);
        return (true, null, new AuthResponse(token, user.Login, user.Email, user.EmailConfirmed, user.Role));
    }
}
