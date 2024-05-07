namespace FSH.WebApi.Application.TeacherGroup.TeacherTeams;
public class TeacherTeamDto : IDto
{
    public Guid Id { get; set; }
    public Guid? TeacherId { get; set; }
    public string TeacherName { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
