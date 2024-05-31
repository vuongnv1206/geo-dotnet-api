using FSH.WebApi.Domain.Question;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSH.WebApi.Domain.Examination;
public class SubmitPaperDetail
{
    public Guid SubmitPaperId { get; set; }
    public Guid QuestionId { get; set; }
    public string? AnswerRaw { get; set; }
    public virtual SubmitPaper? SubmitPaper { get; set; }
    public virtual Question.Question? Question { get; set; }

    public SubmitPaperDetail(Guid submitPaperId, Guid questionId, string? answerRaw)
    {
        SubmitPaperId = submitPaperId;
        QuestionId = questionId;
        AnswerRaw = answerRaw;
    }
}