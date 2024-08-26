using FSH.WebApi.Domain.Examination.Enums;
using System.ComponentModel.DataAnnotations.Schema;

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
    public string? DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public string? DeviceType { get; set; }
    public string? PublicIp { get; set; }
    public string? LocalIp { get; set; }
    public bool canResume { get; set; }

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

    public void MarkAnswer(SubmitPaperDetail submitPaperDetail, float mark)
    {
        var answer = SubmitPaperDetails
            .FirstOrDefault(x => x.SubmitPaperId == submitPaperDetail.SubmitPaperId
                                && x.QuestionId == submitPaperDetail.QuestionId);

        if (answer == null)
        {
            return;
        }
        else
        {
            TotalMark -= answer.Mark ?? 0;
            answer.Mark = mark;
            TotalMark += mark;
        }
    }

    public float getScore(int totalSubmitted)
    {
        float totalMark = 0;

        if (SubmitPaperDetails == null || SubmitPaperDetails.Count == 0)
        {
            return 0;
        }

        if (Paper.ShowMarkResult == ShowResult.No)
        {
            return 0;
        }
        else if (Paper.ShowMarkResult == ShowResult.WhenSubmitted)
        {
            foreach (var item in SubmitPaperDetails)
            {
                if (item.Mark.HasValue)
                {
                    totalMark += item.Mark.Value;
                }
            }
        }
        else
        {
            int totalStudent = Paper.GetTotalStudentsNeedTake();

            if (totalStudent == totalSubmitted || Paper.EndTime < DateTime.UtcNow)
            {
                foreach (var item in SubmitPaperDetails)
                {
                    if (item.Mark.HasValue)
                    {
                        totalMark += item.Mark.Value;
                    }
                }
            }
            else
            {
                return -1;
            }
        }


        return totalMark;
    }

    public bool CheckDetailAnswerResult(int totalSubmitted)
    {
        if (Paper.ShowQuestionAnswer == ShowQuestionAnswer.No)
        {
            return false;
        }
        else if (Paper.ShowQuestionAnswer == ShowQuestionAnswer.WhenSubmitted)
        {
            return true;
        }
        else
        {
            int totalStudent = Paper.GetTotalStudentsNeedTake();

            return totalStudent == totalSubmitted || Paper.EndTime < DateTime.UtcNow;
        }
    }
}
