﻿using System.ComponentModel.DataAnnotations.Schema;

namespace FSH.WebApi.Domain.Examination;
public class SubmitPaper : AuditableEntity, IAggregateRoot
{
    public Guid PaperId { get; set; }
    public SubmitPaperStatus Status { get; set; }
    public DateTime StartTime { get; set; } = DateTime.Now;
    public DateTime? EndTime { get; set; }
    public float TotalMark { get; set; }
    [ForeignKey(nameof(PaperId))]
    public virtual Paper? Paper { get; set; }
    public virtual List<SubmitPaperDetail> SubmitPaperDetails { get; set; } = new();

    public SubmitPaper()
    {

    }
    public SubmitPaper(Guid paperId)
    {
        PaperId = paperId;
    }

    public SubmitPaper(Guid paperId, SubmitPaperStatus status)
    {
        PaperId = paperId;
        Status = status;
    }

    public void SubmitAnswerRaw(SubmitPaperDetail submitAnswer)
    {
        var answer = SubmitPaperDetails
            .FirstOrDefault(x => x.SubmitPaperId == submitAnswer.SubmitPaperId
                                && x.QuestionId == submitAnswer.QuestionId);

        if (answer == null)
        {
            SubmitPaperDetails.Add(submitAnswer);
        }
        else
        {
            answer.AnswerRaw = submitAnswer.AnswerRaw;
        }
    }

    public void MarkAnswer(SubmitPaperDetail submitPaperDetail)
    {
        var answer = SubmitPaperDetails
            .FirstOrDefault(x => x.SubmitPaperId == submitPaperDetail.SubmitPaperId
                                && x.QuestionId == submitPaperDetail.QuestionId);

        if (answer == null) return;
        else answer.Mark = submitPaperDetail.Mark;
    }


}
