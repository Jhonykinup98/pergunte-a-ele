using Microsoft.AspNetCore.Mvc;
using PergunteAele.Api.Models.DTOs;
using PergunteAele.Api.Services;

namespace PergunteAele.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var (success, error) = await _authService.RegisterAsync(request);
        if (!success)
            return BadRequest(new { message = error });

        return Ok(new { message = "Cadastro realizado. Verifique seu e-mail para confirmar a conta." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var (success, error, response) = await _authService.LoginAsync(request);
        if (!success)
            return Unauthorized(new { message = error });

        return Ok(response);
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
    {
        var (success, error) = await _authService.ConfirmEmailAsync(token);
        if (!success)
            return BadRequest(new { message = error });

        return Ok(new { message = "E-mail confirmado com sucesso!" });
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin(GoogleLoginRequest request)
    {
        var (success, error, response) = await _authService.GoogleLoginAsync(request.IdToken);
        if (!success)
            return Unauthorized(new { message = error });

        return Ok(response);
    }
}
