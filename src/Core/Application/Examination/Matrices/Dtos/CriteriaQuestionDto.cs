using FSH.WebApi.Domain.Question.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.Matrices;
public class CriteriaQuestionDto
{
    public Guid QuestionLabelId { get; set; }
    public QuestionType QuestionType { get; set; }
    public int NumberOfQuestion { get; set; }
    public string? RawIndex { get; set; }
}
