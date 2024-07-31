namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class StudentInfo
{
    public Guid Id { get; set; }
    public Guid? StudentId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Guid? ClassId { get; set; }
    public string? ClassName { get; set; }
}
