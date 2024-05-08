using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Application.TeacherGroup.PermissionClasses;

namespace FSH.WebApi.Host.Controllers.TeacherGroup;
public class PermissionClassesController : VersionedApiController
{

    [HttpGet("{id:guid}")]
    [MustHavePermission(FSHAction.View, FSHResource.GroupTeachers)]
    [OpenApiOperation("Get groupTeacher details with permission class", "")]
    public Task<GroupTeacherDto> GetAsync(Guid id)
    {
        return Mediator.Send(new GetGroupTeacherWithPermissionRequest(id));
    }

    [HttpPost("group-permission-in-class")]
    [MustHavePermission(FSHAction.Update, FSHResource.GroupTeachers)]
    [OpenApiOperation("Set a teacher's permissions for class", "")]
    public Task SetTeacherPermissionInClass(BulkUpdateGroupPermissionInClassRequest request)
    {
        return Mediator.Send(request);
    }
}
