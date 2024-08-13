using FSH.WebApi.Application.Notifications;
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
    private readonly INotificationService _notificationService;

    public AcceptRequestJoinTeamRequestHandler(
        ICurrentUser currentUser,
        IRepository<JoinTeacherTeamRequest> joinTeacherTeamRepo,
        IStringLocalizer<AcceptRequestJoinTeamRequestHandler> t,
        IMediator mediator,
        INotificationService notificationService)
    {
        _currentUser = currentUser;
        _joinTeacherTeamRepo = joinTeacherTeamRepo;
        _t = t;
        _mediator = mediator;
        _notificationService = notificationService;
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

        var noti = new BasicNotification
        {
            Message = $"{_currentUser.GetUserEmail()} accepted request. Now you can join \"{_currentUser.GetUserEmail()}\" team",
            Label = BasicNotification.LabelType.Success,
            Title = "Join group",
        };

        await _notificationService.SendNotificationToUser(joinRequest.CreatedBy.ToString(), noti, null, cancellationToken);


        return joinRequest.Id;
    }
}
