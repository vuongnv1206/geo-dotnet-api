using FSH.WebApi.Application.Questions.QuestionLabel.Dtos;
using FSH.WebApi.Domain.Question.Enums;

namespace FSH.WebApi.Application.Questions.Dtos;
public class CreateQuestionDto
{
    public string? Content { get; set; }
    public string? Image { get; set; }
    public string? Audio { get; set; }
    public DefaultIdType? QuestionFolderId { get; set; }
    public QuestionType? QuestionType { get; set; }
    public DefaultIdType? QuestionLabelId { get; set; }
    public QuestionLabelDto? QuestionLable { get; set; }
    public DefaultIdType? QuestionParentId { get; set; }
    public List<CreateQuestionDto>? QuestionPassages { get; set; }
    public List<CreateAnswerDto>? Answers { get; set; }
}
