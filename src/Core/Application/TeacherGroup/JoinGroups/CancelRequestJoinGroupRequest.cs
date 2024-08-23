using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinGroups;
public class CancelRequestJoinGroupRequest : IRequest<Guid>
{
    public Guid RequestId { get; set; }
}

public class CancelRequestJoinGroupRequestHandler : IRequestHandler<CancelRequestJoinGroupRequest, Guid>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<JoinGroupTeacherRequest> _joinGroupTeacherRepo;
    private readonly IStringLocalizer _t;

    public CancelRequestJoinGroupRequestHandler(
        ICurrentUser currentUser,
        IRepository<JoinGroupTeacherRequest> joinGroupTeacherRepo,
        IStringLocalizer<CancelRequestJoinGroupRequestHandler> t)
    {
        _currentUser = currentUser;
        _joinGroupTeacherRepo = joinGroupTeacherRepo;
        _t = t;
    }

    public async Task<DefaultIdType> Handle(CancelRequestJoinGroupRequest request, CancellationToken cancellationToken)
    {
        var spec = new JoinGroupRequestById(request.RequestId);
        var joinRequest = await _joinGroupTeacherRepo.FirstOrDefaultAsync(spec, cancellationToken)
            ?? throw new NotFoundException(_t["Request {0} Not Found", request.RequestId]);

        var userId = _currentUser.GetUserId();
        if (userId != joinRequest.CreatedBy
        || joinRequest.Status != JoinTeacherGroupStatus.Pending)
        {
            throw new ForbiddenException(_t["You can not cancel request."]);
        }

        joinRequest.CancelRequest();

        await _joinGroupTeacherRepo.UpdateAsync(joinRequest);

        return joinRequest.Id;
    }
}
