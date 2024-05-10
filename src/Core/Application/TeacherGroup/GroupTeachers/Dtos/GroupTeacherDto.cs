using FSH.WebApi.Application.TeacherGroup.PermissionClasses;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class GroupTeacherDto : IDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<GroupPermissionInClassDto> GroupPermissionInClasses { get; set; }
}
