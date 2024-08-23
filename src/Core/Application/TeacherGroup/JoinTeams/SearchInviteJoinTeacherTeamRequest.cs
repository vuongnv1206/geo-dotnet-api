using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;
using FSH.WebApi.Domain.TeacherGroup;
using Mapster;

namespace FSH.WebApi.Application.TeacherGroup.JoinTeams;
public class SearchInviteJoinTeacherTeamRequest : IRequest<List<InviteJoinTeacherTeamDto>>
{
}


public class SearchInviteJoinTeacherTeamRequestHandler : IRequestHandler<SearchInviteJoinTeacherTeamRequest, List<InviteJoinTeacherTeamDto>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<InviteJoinTeacherTeam> _inviteJoinRepo;
    private readonly IRepository<TeacherTeam> _teacherTeamRepo;
    private readonly IRepository<JoinTeacherTeamRequest> _joinTeacherTeamRequestRepo;
    private readonly IUserService _userService;

    public SearchInviteJoinTeacherTeamRequestHandler(
        ICurrentUser currentUser,
        IRepository<InviteJoinTeacherTeam> inviteJoinRepo,
        IRepository<TeacherTeam> teacherTeamRepo,
        IRepository<JoinTeacherTeamRequest> joinTeacherTeamRequestRepo,
        IUserService userService)
    {
        _currentUser = currentUser;
        _inviteJoinRepo = inviteJoinRepo;
        _teacherTeamRepo = teacherTeamRepo;
        _joinTeacherTeamRequestRepo = joinTeacherTeamRequestRepo;
        _userService = userService;
    }

    public async Task<List<InviteJoinTeacherTeamDto>> Handle(SearchInviteJoinTeacherTeamRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        var spec = new SearchInviteJoinTeamSpec(userId);
        List<InviteJoinTeacherTeam> invitations = await _inviteJoinRepo.ListAsync(spec, cancellationToken);

        // danh sách lời mời mà email đã tồn tại trong team
        var existEmailsInTeam = new List<InviteJoinTeacherTeam>();

        foreach (var invitation in invitations)
        {
            var specContact = new TeacherTeamByContactSpec(invitation.RecipientEmail, userId);
            if (await _teacherTeamRepo.AnyAsync(specContact, cancellationToken))
            {
                existEmailsInTeam.Add(invitation);
            }
        }

        // danh sách lời mời mà email khong tồn tại trong team
        var invitationNotJoinTeam = invitations.Except(existEmailsInTeam).ToList();

        var datas = invitationNotJoinTeam.Adapt<List<InviteJoinTeacherTeamDto>>();

        // lấy ra danh sách join request team mà có lời mời
        var specRequest = new JoinTeamRequestReceivedInvitationSpec(userId);
        var requestJoins = await _joinTeacherTeamRequestRepo.ListAsync(specRequest, cancellationToken);

        foreach (var item in datas)
        {
            var requestJoin = requestJoins.FirstOrDefault(x => x.InvitationId == item.Id);

            item.Status = requestJoin switch
            {
                null => InvitationStatus.NotRequest,
                { Status: JoinTeacherGroupStatus.Rejected } => InvitationStatus.BeRejected,
                { Status: JoinTeacherGroupStatus.Accepted } => InvitationStatus.BeAccepted,
                _ => InvitationStatus.Requested
            };

            var recipient = await _userService.GetUserDetailByEmailAsync(item.RecipientEmail, cancellationToken);
            item.IsRegistered = recipient.Email != null;
        }

        return datas.OrderByDescending(x => x.CreateOn).ToList();
    }
}
