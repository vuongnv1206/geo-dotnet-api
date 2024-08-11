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

    [HttpPost("accept-request")]
    [MustHavePermission(FSHAction.Create, FSHResource.GroupTeachers)]
    [OpenApiOperation("Accept request join to group.", "")]
    public Task AcceptRequest(AcceptRequestJoinGroupRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("reject-request")]
    [MustHavePermission(FSHAction.Create, FSHResource.GroupTeachers)]
    [OpenApiOperation("Reject request join to group.", "")]
    public Task RejectRequest(RejectRequestJoinGroupRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("cancel-request")]
    [MustHavePermission(FSHAction.Create, FSHResource.GroupTeachers)]
    [OpenApiOperation("Cancel request join to group.", "")]
    public Task CancelRequest(CancelRequestJoinGroupRequest request)
    {
        return Mediator.Send(request);
    }
}
