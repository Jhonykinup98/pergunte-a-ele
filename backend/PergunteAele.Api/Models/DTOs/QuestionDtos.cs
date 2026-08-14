namespace PergunteAele.Api.Models.DTOs;

public record CreateQuestionRequest(string Content);
public record AnswerQuestionRequest(string AnswerContent);

public record QuestionResponse(
    Guid Id, string UserLogin, string Content,
    string? AnswerContent, DateTime CreatedAt, DateTime? AnsweredAt
);