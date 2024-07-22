namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class ClassroomInfoStatistic : IDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? GroupClassId { get; set; }
    public string? GroupClassName { get; set; }
    public int TotalRegister { get; set; }
    public int TotalTested { get; set; }
}
