using FSH.WebApi.Application.Notifications;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinGroups;
public class RejectRequestJoinGroupRequest : IRequest<Guid>
{
    public Guid RequestId { get; set; }
}

public class RejectRequestJoinGroupRequestHandler : IRequestHandler<RejectRequestJoinGroupRequest, Guid>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<JoinGroupTeacherRequest> _joinGroupTeacherRepo;
    private readonly IStringLocalizer _t;
    private readonly INotificationService _notificationService;

    public RejectRequestJoinGroupRequestHandler(
        ICurrentUser currentUser,
        IRepository<JoinGroupTeacherRequest> joinGroupTeacherRepo,
        IStringLocalizer<RejectRequestJoinGroupRequestHandler> t,
        INotificationService notificationService)
    {
        _currentUser = currentUser;
        _joinGroupTeacherRepo = joinGroupTeacherRepo;
        _t = t;
        _notificationService = notificationService;
    }

    public async Task<DefaultIdType> Handle(RejectRequestJoinGroupRequest request, CancellationToken cancellationToken)
    {
        var spec = new JoinGroupRequestById(request.RequestId);
        var joinRequest = await _joinGroupTeacherRepo.FirstOrDefaultAsync(spec, cancellationToken)
            ?? throw new NotFoundException(_t["Request {0} Not Found", request.RequestId]);

        var userId = _currentUser.GetUserId();
        if (userId != joinRequest.ReceiverId
        || joinRequest.Status != JoinTeacherGroupStatus.Pending)
        {
            throw new ForbiddenException(_t["You can not reject request."]);
        }

        joinRequest.RejectRequest();

        await _joinGroupTeacherRepo.UpdateAsync(joinRequest);

        var noti = new BasicNotification
        {
            Message = $"{_currentUser.GetUserEmail()} rejected your join group request. You can send request again.",
            Label = BasicNotification.LabelType.Warning,
            Title = "Join Group",
        };

        await _notificationService.SendNotificationToUser(joinRequest.CreatedBy.ToString(), noti, null, cancellationToken);


        return joinRequest.Id;
    }
}