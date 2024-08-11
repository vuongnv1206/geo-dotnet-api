using FSH.WebApi.Application.TeacherGroup.TeacherInGroups;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinGroups;
public class AcceptRequestJoinGroupRequest : IRequest<Guid>
{
    public Guid RequestId { get; set; }
}

public class AcceptRequestJoinGroupRequestHandler : IRequestHandler<AcceptRequestJoinGroupRequest, Guid>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<JoinGroupTeacherRequest> _joinGroupTeacherRepo;
    private readonly IStringLocalizer _t;
    private readonly IMediator _mediator;

    public AcceptRequestJoinGroupRequestHandler(
        ICurrentUser currentUser,
        IRepository<JoinGroupTeacherRequest> joinGroupTeacherRepo,
        IStringLocalizer<AcceptRequestJoinGroupRequestHandler> t,
        IMediator mediator)
    {
        _currentUser = currentUser;
        _joinGroupTeacherRepo = joinGroupTeacherRepo;
        _t = t;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(AcceptRequestJoinGroupRequest request, CancellationToken cancellationToken)
    {
        var spec = new JoinGroupRequestById(request.RequestId);
        var joinRequest = await _joinGroupTeacherRepo.FirstOrDefaultAsync(spec, cancellationToken)
            ?? throw new NotFoundException(_t["Request {0} Not Found", request.RequestId]);

        var userId = _currentUser.GetUserId();
        if (userId != joinRequest.ReceiverId
        || joinRequest.Status != JoinTeacherGroupStatus.Pending)
        {
            throw new ForbiddenException(_t["You can not accept request."]);
        }

        await _mediator.Send(
        new AddTeacherIntoGroupRequest
        {
            GroupId = joinRequest.GroupId,
            TeacherId = joinRequest.TeacherId,
        },
        cancellationToken);

        joinRequest.AcceptRequest();

        await _joinGroupTeacherRepo.UpdateAsync(joinRequest);

        return joinRequest.Id;
    }
}
