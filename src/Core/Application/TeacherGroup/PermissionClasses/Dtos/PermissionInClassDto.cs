using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.PermissionClasses;
public class PermissionInClassDto
{
    public Guid ClassId { get; set; }
    public PermissionType PermissionType { get; set; }
}
