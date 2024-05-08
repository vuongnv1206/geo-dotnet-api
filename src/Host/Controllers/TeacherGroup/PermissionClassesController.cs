using FSH.WebApi.Application.TeacherGroup.PermissionClasses;

namespace FSH.WebApi.Host.Controllers.TeacherGroup;
public class PermissionClassesController : VersionedApiController
{
    [HttpPost("teacher-permission-in-class")]
    [MustHavePermission(FSHAction.Update, FSHResource.GroupTeachers)]
    [OpenApiOperation("Set a teacher's permissions for class", "")]
    public Task SetTeacherPermissionInClass(BulkUpdateTeacherPermissionInClassRequest request)
    {
        return Mediator.Send(request);
    }
}
