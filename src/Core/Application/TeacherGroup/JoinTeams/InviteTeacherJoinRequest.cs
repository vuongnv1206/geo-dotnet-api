using FSH.WebApi.Application.Common.Mailing;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.Notifications;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinTeams;
public class InviteTeacherJoinRequest : IRequest<Guid>
{
    public string Contact { get; set; } = default!;
}

public class InviteTeacherJoinRequestHandler : IRequestHandler<InviteTeacherJoinRequest, Guid>
{
    private readonly IStringLocalizer _t;
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<InviteJoinTeacherTeam> _inviteJoinRepo;
    private readonly IRepository<TeacherTeam> _teacherTeamRepo;
    private readonly IMailService _mailService;
    private readonly IEmailTemplateService _templateService;
    private readonly IJobService _jobService;
    private readonly INotificationService _notificationService;

    public InviteTeacherJoinRequestHandler(
        IStringLocalizer<InviteTeacherJoinRequestHandler> t,
        IUserService userService,
        ICurrentUser currentUser,
        IRepository<InviteJoinTeacherTeam> inviteJoinRepo,
        IRepository<TeacherTeam> teacherTeamRepo,
        IMailService mailService,
        IEmailTemplateService templateService,
        IJobService jobService,
        INotificationService notificationService)
    {
        _t = t;
        _userService = userService;
        _currentUser = currentUser;
        _inviteJoinRepo = inviteJoinRepo;
        _teacherTeamRepo = teacherTeamRepo;
        _mailService = mailService;
        _templateService = templateService;
        _jobService = jobService;
        _notificationService = notificationService;
    }

    public async Task<DefaultIdType> Handle(InviteTeacherJoinRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        string senderEmail = _currentUser.GetUserEmail();

        if(senderEmail == request.Contact)
        {
            throw new BadRequestException(_t["You cannot join your own group."]);
        }

        var inviteJoin = new InviteJoinTeacherTeam
        {
            RecipientEmail = request.Contact,
            SenderEmail = senderEmail
        };

        var existDuplicateContact = await _teacherTeamRepo.AnyAsync(
            new TeacherTeamByContactSpec(request.Contact, userId), cancellationToken);
        if (existDuplicateContact)
        {
            throw new ConflictException(_t["Teacher's contact exist in team"]);
        }

        await _inviteJoinRepo.AddAsync(inviteJoin);

        var eMailModel = new InviteJoinTeamEmailModel
        {
            RecipientEmail = request.Contact,
            SenderEmail = _currentUser.GetUserEmail(),
            Url = $"http://localhost:5173/invite-join-team/{userId}/{inviteJoin.Id}"
        };

        var mailRequest = new MailRequest(
            new List<string> { request.Contact },
            _t["Join My Team"],
            _templateService.GenerateEmailTemplate("join-my-team", eMailModel));

        _jobService.Enqueue(() => _mailService.SendAsync(mailRequest, CancellationToken.None));


        var recipient = await _userService.GetUserDetailByEmailAsync(request.Contact, cancellationToken);
        if (recipient.Email != null)
        {
            var noti = new BasicNotification
            {
                Message = $"{_currentUser.GetUserEmail()} invites you join their team.",
                Label = BasicNotification.LabelType.Information,
                Title = "Join my team",
                Url = $"/invite-join-team/{userId}/{inviteJoin.Id}"
            };

            await _notificationService.SendNotificationToUser(recipient.Id.ToString(), noti, null, cancellationToken);
        }

        return inviteJoin.Id;
    }
}
