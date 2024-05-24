using FSH.WebApi.Domain.Question.Enums;

namespace FSH.WebApi.Application.Questions.Dtos;


public class QuestionPassageDto : IDto
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public List<AnswerDto>? Answers { get; set; } = new();
}

public class QuestionDto : IDto
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public string? Image { get; set; }
    public string? Audio { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; private set; }
    public Guid LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public QuestionType? QuestionType { get; set; }
    public QuestionFolderDto? QuestionFolder { get; set; }
    public QuestionLableDto? QuestionLable { get; set; }
    public List<QuestionPassageDto>? QuestionPassages { get; set; } = new();
    public List<AnswerDto>? Answers { get; set; } = new();
}