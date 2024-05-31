using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;

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
    [OpenApiOperation("Set a group's permissions for class", "")]
    public Task SetGroupPermissionInClass(BulkUpdateGroupPermissionInClassRequest request)
    {
        return Mediator.Send(request);
    }

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
