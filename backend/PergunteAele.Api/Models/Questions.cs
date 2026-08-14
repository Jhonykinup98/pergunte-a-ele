namespace PergunteAele.Api.Models;

public class Question
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string UserLogin { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? AnswerContent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AnsweredAt { get; set; }
}