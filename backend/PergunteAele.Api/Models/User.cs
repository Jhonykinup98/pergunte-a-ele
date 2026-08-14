namespace PergunteAele.Api.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = "user"; 
    public string? PasswordHash { get; set; }

    public bool EmailConfirmed { get; set; } = false;
    public string? EmailConfirmationToken { get; set; }

    public string Provider { get; set; } = "local"; 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
