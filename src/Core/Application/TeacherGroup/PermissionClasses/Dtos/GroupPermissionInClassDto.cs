

namespace FSH.WebApi.Application.TeacherGroup.PermissionClasses;
public class GroupPermissionInClassDto
{
    public Guid Id { get; set; }
    public Guid GroupTeacherId { get; set; }
    public Guid ClassId { get; set; }
    public string PermissionType { get; set; }
}
