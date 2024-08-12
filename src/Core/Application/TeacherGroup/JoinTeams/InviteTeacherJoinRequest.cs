using FSH.WebApi.Application.Common.Mailing;
using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinTeams;
public class InviteTeacherJoinRequest : IRequest<Guid>
{
    public string Contact { get; set; }
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

    public InviteTeacherJoinRequestHandler(
        IStringLocalizer<InviteTeacherJoinRequestHandler> t,
        IUserService userService,
        ICurrentUser currentUser,
        IRepository<InviteJoinTeacherTeam> inviteJoinRepo,
        IRepository<TeacherTeam> teacherTeamRepo,
        IMailService mailService,
        IEmailTemplateService templateService,
        IJobService jobService)
    {
        _t = t;
        _userService = userService;
        _currentUser = currentUser;
        _inviteJoinRepo = inviteJoinRepo;
        _teacherTeamRepo = teacherTeamRepo;
        _mailService = mailService;
        _templateService = templateService;
        _jobService = jobService;
    }

    public async Task<DefaultIdType> Handle(InviteTeacherJoinRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var inviteJoin = new InviteJoinTeacherTeam
        {
            RecipientEmail = request.Contact,
            SenderEmail = _currentUser.GetUserEmail()
        };

        var existDuplicateContact = await _teacherTeamRepo.AnyAsync(
            new TeacherTeamByContactSpec(request.Contact, userId), cancellationToken);
        if (existDuplicateContact)
        {
            throw new ConflictException(_t["Teacher's contact exist in team"]);
        }

        var recipient = await _userService.GetUserDetailByEmailAsync(request.Contact, cancellationToken);
        inviteJoin.IsRegistered = recipient != null;

        await _inviteJoinRepo.AddAsync(inviteJoin);

        var eMailModel = new InviteJoinTeamEmailModel
        {
            RecipientEmail = request.Contact,
            SenderEmail = _currentUser.GetUserEmail(),
            Url = $"http://localhost:5173/invite-join-team/{userId}"
        };

        var mailRequest = new MailRequest(
            new List<string> { request.Contact },
            _t["Join My Team"],
            _templateService.GenerateEmailTemplate("join-my-team", eMailModel));

        _jobService.Enqueue(() => _mailService.SendAsync(mailRequest, CancellationToken.None));

        return inviteJoin.Id;
    }
}
