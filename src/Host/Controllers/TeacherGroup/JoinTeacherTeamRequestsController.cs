using FSH.WebApi.Application.TeacherGroup.JoinGroups;
using FSH.WebApi.Application.TeacherGroup.JoinTeams;

namespace FSH.WebApi.Host.Controllers.TeacherGroup;

public class JoinTeacherTeamRequestsController : VersionedApiController
{
    [HttpPost("send-request")]
    [MustHavePermission(FSHAction.Create, FSHResource.GroupTeachers)]
    [OpenApiOperation("Send request join to team.", "")]
    public Task<Guid> SendRequest(SendRequestJoinTeamRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("search")]
    [MustHavePermission(FSHAction.Search, FSHResource.GroupTeachers)]
    [OpenApiOperation("Search request join to team.", "")]
    public Task<PaginationResponse<JoinTeacherTeamRequestDto>> SearchAsync(SearchJoinTeacherTeamRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("accept-request")]
    [MustHavePermission(FSHAction.Create, FSHResource.GroupTeachers)]
    [OpenApiOperation("Accept request join to team.", "")]
    public Task AcceptRequest(AcceptRequestJoinTeamRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("reject-request")]
    [MustHavePermission(FSHAction.Create, FSHResource.GroupTeachers)]
    [OpenApiOperation("Reject request join to team.", "")]
    public Task RejectRequest(RejectRequestJoinTeamRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("cancel-request")]
    [MustHavePermission(FSHAction.Create, FSHResource.GroupTeachers)]
    [OpenApiOperation("Cancel request join to team.", "")]
    public Task CancelRequest(CancelRequestJoinTeamRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("invite-teacher")]
    [MustHavePermission(FSHAction.Create, FSHResource.GroupTeachers)]
    [OpenApiOperation("Cancel request join to team.", "")]
    public Task InviteTeacherJoinTeam(InviteTeacherJoinRequest request)
    {
        return Mediator.Send(request);
    }
}
