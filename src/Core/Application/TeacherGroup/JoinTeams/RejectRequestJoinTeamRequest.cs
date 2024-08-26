using FSH.WebApi.Application.Notifications;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinTeams;
public class RejectRequestJoinTeamRequest : IRequest<Guid>
{
    public Guid RequestId { get; set; }
}
public class RejectRequestJoinTeamRequestHandler : IRequestHandler<RejectRequestJoinTeamRequest, Guid>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<JoinTeacherTeamRequest> _joinTeacherTeamRepo;
    private readonly IStringLocalizer _t;
    private readonly INotificationService _notificationService;

    public RejectRequestJoinTeamRequestHandler(
        ICurrentUser currentUser,
        IRepository<JoinTeacherTeamRequest> joinTeacherTeamRepo,
        IStringLocalizer<RejectRequestJoinTeamRequestHandler> t,
        INotificationService notificationService)
    {
        _currentUser = currentUser;
        _joinTeacherTeamRepo = joinTeacherTeamRepo;
        _t = t;
        _notificationService = notificationService;
    }

    public async Task<DefaultIdType> Handle(RejectRequestJoinTeamRequest request, CancellationToken cancellationToken)
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

        joinRequest.RejectRequest();

        await _joinTeacherTeamRepo.UpdateAsync(joinRequest);

        var noti = new BasicNotification
        {
            Message = $"{_currentUser.GetUserEmail()} rejected your join team request. You can send request again.",
            Label = BasicNotification.LabelType.Warning,
            Title = "Join Team",
        };

        await _notificationService.SendNotificationToUser(joinRequest.CreatedBy.ToString(), noti, null, cancellationToken);

        return joinRequest.Id;
    }
}
