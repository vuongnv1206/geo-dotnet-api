

namespace FSH.WebApi.Domain.Examination;
public class SubmitPaperDetail : AuditableEntity, IAggregateRoot
{
    public Guid SubmitPaperId { get; set; }
    public Guid QuestionId { get; set; }
    public string? AnswerRaw { get; set; }
    public float? Mark { get; set; }
    public virtual SubmitPaper? SubmitPaper { get; set; }
    public virtual Question.Question? Question { get; set; }

    public SubmitPaperDetail(Guid submitPaperId, Guid questionId, string? answerRaw)
    {
        SubmitPaperId = submitPaperId;
        QuestionId = questionId;
        AnswerRaw = answerRaw;
    }

}