using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FSH.WebApi.Domain.Examination.Enums;

namespace FSH.WebApi.Domain.Examination;
public class Paper : AuditableEntity, IAggregateRoot
{
    public string ExamName { get; set; } = null!;
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
    public string ExamCode { get; set; } = "Acb";
    public string? Content { get; set; }
    public string? Description { get; set; }
    public virtual PaperLabel? PaperLable { get; set; }
    public virtual PaperFolder? PaperFolder { get; set; }

    public Paper(string examName, PaperStatus status, PaperType type, string? content, string? description, Guid? paperFolderId, string? password)
    {
        ExamName = examName;
        Status = status;
        Type = type;
        Content = content;
        Description = description;
        PaperFolderId = paperFolderId;
        Password = password;
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


    public bool CanUpdate(DefaultIdType userId)
    {
        return CreatedBy == userId;
    }

}
