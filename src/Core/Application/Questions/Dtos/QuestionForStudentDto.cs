using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;

namespace FSH.WebApi.Application.Questions.Dtos;

public class QuestionPassagesForStudentDto : IDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Content { get; set; }
    public List<AnswerForStudentDto>? Answers { get; set; } = new();
}

public class QuestionForStudentDto : AuditableEntity, IDto
{
    public string? Content { get; set; }
    public string? Image { get; set; }
    public string? Audio { get; set; }
    public int? RawIndex { get; set; }
    public float? Mark { get; set; }
    public QuestionType? QuestionType { get; set; }
    public QuestionLableDto? QuestionLable { get; set; }
    public List<QuestionPassagesForStudentDto>? QuestionPassages { get; set; } = new();
    public List<AnswerForStudentDto>? Answers { get; set; } = new();
    public QuestionStatus QuestionStatus { get; set; }
}