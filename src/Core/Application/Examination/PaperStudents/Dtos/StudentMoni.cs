using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.SubmitPapers.Dtos;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperStudents.Dtos;
public class StudentMoni : IDto
{
    public DefaultIdType StudentId { get; set; }
    public UserDetailsDto? Student { get; set; }
    public DefaultIdType PaperId { get; set; }
    public PaperDto? Paper { get; set; }
    public DefaultIdType? SubmitPaperId { get; set; }
    public SubmitPaperDto? SubmitPaper { get; set; }
    public CompletionStatusEnum CompletionStatus { get; set; }
    public bool IsSuspicious { get; set; }
    public List<SubmitPaperLog> SubmitPaperLogs { get; set; }
    public string? ClassId { get; set; }
    public ClassMoniDto? Class { get; set; }
    public StudentMoni()
    {
    }
}
