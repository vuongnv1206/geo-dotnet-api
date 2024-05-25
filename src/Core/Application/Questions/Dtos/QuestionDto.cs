using FSH.WebApi.Domain.Question.Enums;

namespace FSH.WebApi.Application.Questions.Dtos;

public class QuestionPassagesDto : IDto
{
    public string? Content { get; set; }
    public List<AnswerDto>? Answers { get; set; } = new();
}

public class QuestionDto : AuditableEntity, IDto
{
    public string? Content { get; set; }
    public string? Image { get; set; }
    public string? Audio { get; set; }
    public QuestionFolderDto? QuestionFolder { get; set; }
    public QuestionType? QuestionType { get; set; }
    public QuestionLableDto? QuestionLable { get; set; }
    public List<QuestionPassagesDto>? QuestionPassages { get; set; } = new();
    public List<AnswerDto>? Answers { get; set; } = new();
}