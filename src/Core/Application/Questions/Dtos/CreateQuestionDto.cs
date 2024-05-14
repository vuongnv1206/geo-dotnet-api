using FSH.WebApi.Domain.Question.Enums;
using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions;
public class CreateQuestionDto
{
    public string? Content { get; set; }
    public string? Image { get; set; }
    public string? Audio { get; set; }
    public Guid? QuestionFolderId { get; set; }
    public QuestionType? QuestionType { get; set; }
    public Guid? QuestionLableId { get; set; }
    public Guid? ParentId { get; set; }
    public List<CreateQuestionDto>? QuestionPassages { get; set; }
    public List<AnswerDto>? Answers { get; set; }
}
