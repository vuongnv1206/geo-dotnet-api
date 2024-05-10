using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public string ExamCode { get; set; } = null!;

    public virtual PaperLable? PaperLable { get; set; }
    public virtual PaperFolder? PaperFolder { get; set; }
}
