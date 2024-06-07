using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class SubmitPaperDto : IDto
{
    public Guid Id { get; set; }
    public Guid PaperId { get; set; }
    public SubmitPaperStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public float TotalMark { get; set; }
    public Guid CreatedBy { get; set; }
    public string? CreatorName { get; set; }
    public DateTime CreatedOn { get; private set; }
}
