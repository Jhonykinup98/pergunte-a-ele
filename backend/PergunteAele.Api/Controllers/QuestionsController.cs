using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PergunteAele.Api.Data;
using PergunteAele.Api.Models;
using PergunteAele.Api.Models.DTOs;
using System.Security.Claims;

namespace PergunteAele.Api.Controllers;

[ApiController]
[Route("questions")]
[Authorize]
public class QuestionsController : ControllerBase
{
    private readonly AppDbContext _db;
    public QuestionsController(AppDbContext db) => _db = db;

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool IsAdmin => User.IsInRole("admin");

    [HttpGet]
    public async Task<IActionResult> GetQuestions()
    {
        var questions = await _db.Questions
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => new QuestionResponse(q.Id, q.UserLogin, q.Content, q.AnswerContent, q.CreatedAt, q.AnsweredAt))
            .ToListAsync();

        return Ok(questions);
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuestion(CreateQuestionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest(new { message = "A pergunta não pode estar vazia." });

        var question = new Question
        {
            UserId = CurrentUserId,
            UserLogin = User.FindFirstValue("login") ?? "usuário",
            Content = request.Content
        };

        _db.Questions.Add(question);
        await _db.SaveChangesAsync();

        return Ok(new QuestionResponse(question.Id, question.UserLogin, question.Content, null, question.CreatedAt, null));
    }

    [HttpPost("{id}/answer")]
    public async Task<IActionResult> AnswerQuestion(Guid id, AnswerQuestionRequest request)
    {
        if (!IsAdmin) return Forbid();

        var question = await _db.Questions.FindAsync(id);
        if (question is null) return NotFound();

        question.AnswerContent = request.AnswerContent;
        question.AnsweredAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new QuestionResponse(question.Id, question.UserLogin, question.Content, question.AnswerContent, question.CreatedAt, question.AnsweredAt));
    }
}