using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class GroupTeacherDto : IDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public List<GroupPermissionInClassDto>? GroupPermissionInClasses { get; set; }
    public List<TeacherTeamDto>? TeacherTeams { get; set; }
}
