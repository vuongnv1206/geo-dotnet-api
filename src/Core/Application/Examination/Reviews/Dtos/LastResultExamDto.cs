
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.SubmitPapers;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Reviews;
public class LastResultExamDto
{
    public Guid Id { get; set; }
    public Guid PaperId { get; set; }
    public SubmitPaperStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public float TotalMark { get; set; }
    public int TotalQuestion { get; set; }
    public PaperDto Paper { get; set; }
    public List<SubmitPaperDetailDto> SubmitPaperDetails { get; set; }
    public UserDetailsDto Student { get; set; }

}
