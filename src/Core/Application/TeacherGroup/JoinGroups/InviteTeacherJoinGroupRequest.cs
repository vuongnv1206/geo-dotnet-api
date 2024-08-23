using FSH.WebApi.Application.Notifications;
using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams.Specs;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinGroups;
public class InviteTeacherJoinGroupRequest : IRequest<Guid>
{
    public Guid TeacherId { get; set; } // id của bảng TeacherTeam
    public Guid GroupId { get; set; }
}

public class InviteTeacherJoinGroupRequestHandler : IRequestHandler<InviteTeacherJoinGroupRequest, Guid>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<TeacherTeam> _teacherTeamRepo;
    private readonly INotificationService _notificationService;
    private readonly IStringLocalizer _t;
    private readonly IRepository<GroupTeacher> _groupRepo;

    public InviteTeacherJoinGroupRequestHandler(
        ICurrentUser currentUser,
        IRepository<TeacherTeam> teacherTeamRepo,
        INotificationService notificationService,
        IStringLocalizer<InviteTeacherJoinGroupRequestHandler> t,
        IRepository<GroupTeacher> groupRepo)
    {
        _currentUser = currentUser;
        _teacherTeamRepo = teacherTeamRepo;
        _notificationService = notificationService;
        _t = t;
        _groupRepo = groupRepo;
    }

    public async Task<DefaultIdType> Handle(InviteTeacherJoinGroupRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        var specTeacher = new TeacherTeamByIdSpec(request.TeacherId, userId);
        var teacher = await _teacherTeamRepo.FirstOrDefaultAsync(specTeacher, cancellationToken)
            ?? throw new NotFoundException(_t["Teacher {0} Not Found in your team", request.TeacherId]);

        var specGroup = new GroupTeacherByIdSpec(request.GroupId);
        var group = await _groupRepo.FirstOrDefaultAsync(specGroup, cancellationToken)
            ?? throw new NotFoundException(_t["Group {0} Not Found.", request.GroupId]);

        if (group.TeacherInGroups.Any(x => x.TeacherTeamId == request.TeacherId))
        {
            throw new BadRequestException(_t["Teacher {0} are already in group", teacher.TeacherName]);
        }

        if (teacher.TeacherId == null)
        {
            throw new BadRequestException(_t["Teacher {0} has not been registered.", teacher.TeacherName]);
        }

        var noti = new BasicNotification
        {
            Message = $"{_currentUser.GetUserEmail()} invites you join \"{group.Name}\".",
            Label = BasicNotification.LabelType.Information,
            Title = "Join my group",
            Url = $"/join-group/{request.GroupId}"
        };

        await _notificationService.SendNotificationToUser(teacher.TeacherId.ToString(), noti, null, cancellationToken);

        return default(DefaultIdType);
    }
}
