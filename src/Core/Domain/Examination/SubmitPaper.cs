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
}
