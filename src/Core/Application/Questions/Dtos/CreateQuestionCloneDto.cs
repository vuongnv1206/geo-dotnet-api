using FSH.WebApi.Domain.Question.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Questions.Dtos;
public class CreateQuestionCloneDto
{
    public string? Content { get; set; }
    public string? Image { get; set; }
    public string? Audio { get; set; }
    public Guid? QuestionFolderId { get; set; }
    public QuestionType? QuestionType { get; set; }
    public Guid? QuestionLabelId { get; set; }
    public Guid? QuestionParentId { get; set; }
    public Guid? OriginalQuestionId { get; set; }
    public List<CreateQuestionCloneDto>? QuestionPassages { get; set; }
    public List<CreateAnswerDto>? Answers { get; set; }
}
