using FSH.WebApi.Application.TeacherGroup.JoinGroups;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinTeams;
public class AcceptRequestJoinTeamRequest : IRequest<Guid>
{
    public Guid RequestId { get; set; }
    public string? Nickname { get; set; }
}
public class AcceptRequestJoinTeamRequestHandler : IRequestHandler<AcceptRequestJoinTeamRequest, Guid>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<JoinTeacherTeamRequest> _joinTeacherTeamRepo;
    private readonly IStringLocalizer _t;
    private readonly IMediator _mediator;

    public AcceptRequestJoinTeamRequestHandler(
        ICurrentUser currentUser,
        IRepository<JoinTeacherTeamRequest> joinTeacherTeamRepo,
        IStringLocalizer<AcceptRequestJoinTeamRequestHandler> t,
        IMediator mediator)
    {
        _currentUser = currentUser;
        _joinTeacherTeamRepo = joinTeacherTeamRepo;
        _t = t;
        _mediator = mediator;
    }

    public async Task<DefaultIdType> Handle(AcceptRequestJoinTeamRequest request, CancellationToken cancellationToken)
    {
        var spec = new JoinTeacherTeamByIdSpec(request.RequestId);
        var joinRequest = await _joinTeacherTeamRepo.FirstOrDefaultAsync(spec, cancellationToken)
            ?? throw new NotFoundException(_t["Request {0} Not Found", request.RequestId]);

        var userId = _currentUser.GetUserId();
        if (userId != joinRequest.AdminTeamId
        || joinRequest.Status != JoinTeacherGroupStatus.Pending)
        {
            throw new ForbiddenException(_t["You can not accept request."]);
        }

        await _mediator.Send(
        new AddTeacherIntoTeacherTeamRequest
        {
            Contact = joinRequest.SenderEmail,
            TeacherName = string.IsNullOrEmpty(request.Nickname) ? joinRequest.SenderEmail : request.Nickname,
        });

        joinRequest.AcceptRequest();

        await _joinTeacherTeamRepo.UpdateAsync(joinRequest);

        return joinRequest.Id;
    }
}
