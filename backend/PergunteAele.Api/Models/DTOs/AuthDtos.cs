namespace PergunteAele.Api.Models.DTOs;

public record RegisterRequest(
    string Login,
    string Email,
    string Password,
    string ConfirmPassword
);

public record LoginRequest(
    string Login,
    string Password
);

public record GoogleLoginRequest(
    string IdToken 
);

public record ConfirmEmailRequest(
    string Token
);

public record AuthResponse(
    string Token, string Login, string Email, bool EmailConfirmed, string Role
);
