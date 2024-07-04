using FSH.WebApi.Application.Examination.PaperLabels;
using FSH.WebApi.Application.Subjects;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class PaperStudentDto : IDto
{
    public Guid Id { get; set; }
    public string ExamName { get; set; }
    public int NumberOfQuestion { get; set; }
    public int TotalAttended { get; set; }
    public int? Duration { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public ShowResult ShowMarkResult { get; set; }
    public ShowQuestionAnswer ShowQuestionAnswer { get; set; }
    public string Type { get; set; }
    public bool IsPublish { get; set; }
    public string ExamCode { get; set; }
    public string? Description { get; set; }
    public Guid? PaperLabelId { get; set; }
    public Guid? SubjectId { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatorName { get; set; }
    public DateTime CreatedOn { get; private set; }
    public PaperLabelDto PaperLable { get; set; }
    public SubjectDto Subject { get; set; }
}
