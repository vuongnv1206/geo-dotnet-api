using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperStudents;
public class StudentTestHistoryDto : IDto
{
    public Guid Id { get; set; }
    public string ExamName { get; set; }
    public int? Duration { get; set; }
    public Guid? PaperLabelId { get; set; }
    public string? PaperLabelName { get; set; }
    public Guid? SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public DateTime? StartedTime { get; set; }
    public DateTime? SubmittedTime { get; set; }
    public float? Score { get; set; }
    public CompletionStatusEnum CompletionStatus { get; set; }
    public ShowResult ShowMarkResult { get; set; }
}
