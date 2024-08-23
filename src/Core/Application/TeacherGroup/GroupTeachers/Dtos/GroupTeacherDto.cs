using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class GroupTeacherDto : IDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? QrCode { get; set; }
    public string? JoinLink { get; set; }
    public string? AdminGroup { get; set; }
    public Guid CreatedBy { get; set; }
    public List<GroupPermissionInClassDto>? GroupPermissionInClasses { get; set; }
    public List<TeacherTeamDto>? TeacherTeams { get; set; }
}
