using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions.Dtos;

public class AnswerDto : IDto
{
    public string? Content { get; set; }
    public Guid? QuestionId { get; set; }
    public bool IsCorrect { get; set; }
}