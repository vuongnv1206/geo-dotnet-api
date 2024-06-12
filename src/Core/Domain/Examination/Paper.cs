using FSH.WebApi.Domain.Examination.Enums;
using FSH.WebApi.Domain.Subjects;

namespace FSH.WebApi.Domain.Examination;
public class Paper : AuditableEntity, IAggregateRoot
{
    //General config
    public string ExamName { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? Duration { get; set; }
    public PaperType Type { get; set; }
    public bool IsPublish { get; set; }
    public string? Content { get; set; }
    public string? Description { get; set; }
    public PaperStatus Status { get; set; }
    //Security
    public bool Shuffle { get; set; }
    public bool ShowMarkResult { get; set; }
    public bool ShowQuestionAnswer { get; set; }
    public string? Password { get; set; }
    public string ExamCode { get; set; }
    public int NumberAttempt { get; set; }

    //Navigation
    public Guid? PaperLabelId { get; set; }
    public Guid? PaperFolderId { get; set; }
    public Guid? SubjectId { get; set; }
    public virtual Subject? Subject { get; set; }
    public virtual PaperLabel? PaperLabel { get; set; }
    public virtual PaperFolder? PaperFolder { get; set; }
    public virtual List<PaperQuestion> PaperQuestions { get; set; } = new();
    public virtual List<SubmitPaper> SubmitPapers { get; set; } = new();

    public Paper(
        string examName,
        PaperStatus status,
        PaperType type,
        string? content,
        string? description,
        string? password,
        Guid? paperFolderId,
        Guid? paperLabelId,
        Guid? subjectId)
    {
        ExamName = examName;
        Status = status;
        Type = type;
        Content = content;
        Description = description;
        PaperFolderId = paperFolderId;
        PaperLabelId = paperLabelId;
        SubjectId = subjectId;
        Password = password;
        ExamCode = examName;
    }

    public void AddQuestions(List<PaperQuestion> questions)
    {
        foreach (var q in questions)
        {
            if (PaperQuestions.Any(x => x.QuestionId == q.QuestionId))
                continue;

            PaperQuestions.Add(new PaperQuestion
            {
                QuestionId = q.QuestionId,
                PaperId = Id,
                Mark = q.Mark,
                RawIndex = q.RawIndex
            });
        }
    }
    public void UpdateQuestions(List<PaperQuestion> questions)
    {
        PaperQuestions.RemoveAll(pq => !questions.Any(q => q.QuestionId == pq.QuestionId));
        foreach (var q in questions)
        {
            var existingPaperQuestion = PaperQuestions.FirstOrDefault(pq => pq.QuestionId == q.QuestionId);
            if (existingPaperQuestion != null)
            {
                existingPaperQuestion.Mark = q.Mark;
                existingPaperQuestion.RawIndex = q.RawIndex;
            }
            else
            {
                PaperQuestions.Add(new PaperQuestion
                {
                    QuestionId = q.QuestionId,
                    PaperId = this.Id,
                    Mark = q.Mark,
                    RawIndex = q.RawIndex
                });
            }
        }
    }

    public Paper Update(
        string examName,
        PaperStatus status,
        DateTime? startTime,
        DateTime? endTime,
        int? duration,
        bool shuffle,
        bool showMarkResult,
        bool showQuestionAnswer,
        PaperType type,
        bool isPublish,
        string? content,
        string? description,
        string? password,
        Guid? paperFolderId,
        Guid? paperLabelId,
        Guid? subjectId)
    {
        ExamName = examName;
        Status = status;
        StartTime = startTime;
        EndTime = endTime;
        Duration = duration;
        Shuffle = shuffle;
        ShowMarkResult = showMarkResult;
        ShowQuestionAnswer = showQuestionAnswer;
        Type = type;
        IsPublish = isPublish;
        Content = content;
        Description = description;
        Password = password;
        PaperLabelId = paperLabelId;
        SubjectId = subjectId;
        PaperFolderId = paperFolderId;
        return this;
    }

    public bool CanUpdate(DefaultIdType userId)
    {
        return CreatedBy == userId;
    }
}
