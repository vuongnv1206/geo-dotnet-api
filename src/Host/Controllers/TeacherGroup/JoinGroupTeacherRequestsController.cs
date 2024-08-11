using FSH.WebApi.Application.TeacherGroup.JoinGroups;

namespace FSH.WebApi.Host.Controllers.TeacherGroup;

public class JoinGroupTeacherRequestsController : VersionedApiController
{
    [HttpPost("search")]
    [MustHavePermission(FSHAction.Search, FSHResource.GroupTeachers)]
    [OpenApiOperation("Search request join to group.", "")]
    public Task<PaginationResponse<JoinGroupTeacherRequestDto>> SearchAsync(SearchJoinGroupTeacherRequest request)
    {
        return Mediator.Send(request);
    }
}
