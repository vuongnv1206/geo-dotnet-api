namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class PaperInfoStatistic : IDto
{
    public Guid Id { get; set; }
    public string ExamName { get; set; }
    public Guid? PaperLabelId { get; set; }
    public string? PaperLabelName { get; set; }
    public Guid? SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public int TotalRegister { get; set; }
    public int TotalNotComplete { get; set; }
    public int TotalDoing { get; set; }
    public int TotalAttendee { get; set; }
    public float AverageMark { get; set; }
    public int TotalPopular { get; set; }
    public float MarkPopular { get; set; }
    public List<ClassroomInfoStatistic>? Classrooms { get; set; }
}
