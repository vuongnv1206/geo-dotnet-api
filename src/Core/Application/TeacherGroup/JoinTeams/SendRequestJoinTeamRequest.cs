using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinTeams;
public class SendRequestJoinTeamRequest : IRequest<Guid>
{
    public Guid AdminTeamId { get; set; }
    public string? Content { get; set; }
}

public class SendRequestJoinTeamRequestHandler : IRequestHandler<SendRequestJoinTeamRequest, Guid>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<JoinTeacherTeamRequest> _joinTeacherTeamRepo;
    private readonly IStringLocalizer _t;
    private readonly IUserService _userService;
    private readonly IRepository<TeacherTeam> _teacherTeamRepo;

    public SendRequestJoinTeamRequestHandler(
        ICurrentUser currentUser,
        IRepository<JoinTeacherTeamRequest> joinTeacherTeamRepo,
        IStringLocalizer<SendRequestJoinTeamRequestHandler> t,
        IUserService userService,
        IRepository<TeacherTeam> teacherTeamRepo)
    {
        _currentUser = currentUser;
        _joinTeacherTeamRepo = joinTeacherTeamRepo;
        _t = t;
        _userService = userService;
        _teacherTeamRepo = teacherTeamRepo;
    }

    public async Task<DefaultIdType> Handle(SendRequestJoinTeamRequest request, CancellationToken cancellationToken)
    {
        _ = await _userService.GetAsync(request.AdminTeamId.ToString(), cancellationToken)
            ?? throw new NotFoundException(_t["User {0} Not Found."]);

        var useId = _currentUser.GetUserId();
        if (useId == request.AdminTeamId)
        {
            throw new BadRequestException(_t["You cannot join your own team."]);
        }

        var spec = new TeacherTeamByTeacherIdSpec(useId, request.AdminTeamId);
        if (await _teacherTeamRepo.AnyAsync(spec, cancellationToken))
        {
            throw new BadRequestException(_t["You are ready in this team."]);
        }

        var existJoinTeamRequest = await _joinTeacherTeamRepo.ListAsync(
            new ExistJoinRequestTeacherTeamSpec(useId, request.AdminTeamId), cancellationToken);

        if (existJoinTeamRequest != null
            && existJoinTeamRequest.Any(x => x.Status == JoinTeacherGroupStatus.Pending))
        {
            throw new BadRequestException(_t["You have already sent the request before."]);
        }

        string emailSender = await GetEmailUser(useId, cancellationToken);
        var joinRequest = new JoinTeacherTeamRequest(request.AdminTeamId, request.Content, emailSender);
        await _joinTeacherTeamRepo.AddAsync(joinRequest);

        return joinRequest.Id;
    }
    private async Task<string> GetEmailUser(Guid userId, CancellationToken cancellationToken)
    {
        var userDetail = await _userService.GetAsync(userId.ToString(), cancellationToken);

        return userDetail.Email;
    }
}
