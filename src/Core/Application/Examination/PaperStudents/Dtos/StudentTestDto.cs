namespace FSH.WebApi.Application.Examination.PaperStudents;
public class StudentTestDto : IDto
{
    public Guid Id { get; set; }
    public string ExamName { get; set; }
    public DateTime? StartTime { get; set; } // can do whenever from start time to end time
    public DateTime? EndTime { get; set; } // can do whenever from start time to end time
    public int? Duration { get; set; }
    public bool IsPublish { get; set; }
    public string? Description { get; set; }
    public Guid? PaperLabelId { get; set; }
    public string? PaperLabelName { get; set; }
    public Guid? SubjectId { get; set; }
    public string? SubjectName { get; set;}
    public CompletionStatusEnum CompletionStatus { get; set; }
}
