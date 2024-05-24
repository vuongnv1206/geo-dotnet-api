
using FSH.WebApi.Domain.Examination.Enums;

namespace FSH.WebApi.Domain.Examination;
public class Paper : AuditableEntity, IAggregateRoot
{
    public string ExamName { get; set; }
    public PaperStatus Status { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public Guid? PaperLabelId { get; set; }
    public int NumberOfQuestion { get; set; }
    public int? Duration { get; set; }
    public bool Shuffle { get; set; }
    public bool ShowMarkResult { get; set; }
    public bool ShowQuestionAnswer { get; set; }
    public string? Password { get; set; }
    public PaperType Type { get; set; }
    public Guid? PaperFolderId { get; set; }
    public bool IsPublish{ get; set; }
    public string ExamCode { get; set; }
    public string? Content { get; set; }
    public string? Description { get; set; }
    public virtual PaperLabel? PaperLable { get; set; }
    public virtual PaperFolder? PaperFolder { get; set; }
    public virtual List<PaperQuestion> PaperQuestions { get; set; } = new();

    public Paper(
        string examName,
        PaperStatus status,
        PaperType type,
        string? content,
        string? description,
        Guid? paperFolderId,
        string? password)
    {
        ExamName = examName;
        Status = status;
        Type = type;
        Content = content;
        Description = description;
        PaperFolderId = paperFolderId;
        Password = password;
    }

    public void AddQuestions(List<PaperQuestion> questions)
    {
        foreach(var q in questions)
        {
            if (PaperQuestions.Any(x => x.QuestionId == q.Question.Id))
                continue;

            PaperQuestions.Add(new PaperQuestion
            {
                QuestionId = q.QuestionId,
                PaperId = Id,
                Mark = q.Mark
            });
        }
    }

    public Paper Update(string examName,
        PaperStatus status,
        DateTime? startTime,
        DateTime? endTime,
        Guid? paperLabelId,
        int numberOfQuestion,
        int? duration,
        bool shuffle,
        bool showMarkResult,
        bool showQuestionAnswer,
        PaperType type,
        Guid? paperFolderId,
        bool isPublish,
        string? content,
        string? description)
    {
        ExamName = examName;
        Status = status;
        StartTime = startTime;
        EndTime = endTime;
        PaperLabelId = paperLabelId;
        NumberOfQuestion = numberOfQuestion;
        Duration = duration;
        Shuffle = shuffle;
        ShowMarkResult = showMarkResult;
        ShowQuestionAnswer = showQuestionAnswer;
        Type = type;
        PaperFolderId = paperFolderId;
        IsPublish = isPublish;
        Content = content;
        Description = description;

        return this;
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
            }
            else
            {
                PaperQuestions.Add(new PaperQuestion
                {
                    QuestionId = q.QuestionId,
                    PaperId = this.Id,
                    Mark = q.Mark
                });
            }
        }
    }

    public bool CanUpdate(DefaultIdType userId)
    {
        return CreatedBy == userId;
    }

}
