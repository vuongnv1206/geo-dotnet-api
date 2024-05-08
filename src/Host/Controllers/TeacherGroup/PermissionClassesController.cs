using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;

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

    [HttpGet("teacher-permission-in-class/{teacherId:guid}")]
    [MustHavePermission(FSHAction.Search, FSHResource.GroupTeachers)]
    [OpenApiOperation("Get teacher's permissions in class", "")]
    public Task<TeacherTeamDto> GetTeacherPermissionInClass(Guid teacherId)
    {
        return Mediator.Send(new GetTeacherPermissionWithClassRequest(teacherId));
    }
}
