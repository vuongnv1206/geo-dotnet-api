using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.Notifications;
using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Domain.TeacherGroup;
using FSH.WebApi.Shared.Notifications;

namespace FSH.WebApi.Application.TeacherGroup.JoinGroups;
public class SendRequestJoinGroupRequest : IRequest<Guid>
{
    public Guid GroupId { get; set; }
    public string? Content { get; set; }
}

public class SendRequestJoinGroupRequestHandler : IRequestHandler<SendRequestJoinGroupRequest, Guid>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<GroupTeacher> _teacherGroupRepo;
    private readonly IStringLocalizer _t;
    private readonly IRepository<TeacherTeam> _teacherTeamRepo;
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;

    public SendRequestJoinGroupRequestHandler(
        ICurrentUser currentUser,
        IRepository<GroupTeacher> teacherGroupRepo,
        IStringLocalizer<SendRequestJoinGroupRequestHandler> t,
        IRepository<TeacherTeam> teacherTeamRepo,
        IUserService userService,
        INotificationService notificationService)
    {
        _currentUser = currentUser;
        _teacherGroupRepo = teacherGroupRepo;
        _t = t;
        _teacherTeamRepo = teacherTeamRepo;
        _userService = userService;
        _notificationService = notificationService;
    }

    public async Task<DefaultIdType> Handle(SendRequestJoinGroupRequest request, CancellationToken cancellationToken)
    {
        var group = await _teacherGroupRepo
            .FirstOrDefaultAsync(new GroupTeacherByIdSpec(request.GroupId), cancellationToken)
            ?? throw new NotFoundException(_t["Group {0} Not Found", request.GroupId]);

        var userId = _currentUser.GetUserId();

        var fullNameAdminGroup = _userService.GetFullName(group.CreatedBy);
        // kiểm tra giáo viên có trong team của chủ group
        var existTeacherTeam = await _teacherTeamRepo
            .FirstOrDefaultAsync(new ExistTeacherMemberInTeamSpec(group.CreatedBy, userId))
            ?? throw new BadRequestException(_t["You can not in {0} team.", fullNameAdminGroup.Result]);

        if (group.TeacherInGroups.Any(x => x.TeacherTeamId == existTeacherTeam.Id))
        {
            throw new BadRequestException(_t["You are already in the group."]);
        }

        if (group.JoinGroupRequests.Any(x => x.CreatedBy == userId
                && x.Status == JoinTeacherGroupStatus.Pending))
        {
            throw new BadRequestException(_t["You are already send request."]);
        }

        group.AddRequestJoinGroup(new JoinGroupTeacherRequest(request.GroupId, existTeacherTeam.Id, group.CreatedBy, request.Content));

        await _teacherGroupRepo.UpdateAsync(group);

        string userEmail = _currentUser.GetUserEmail();
        var noti = new BasicNotification
        {
            Message = $"{userEmail} want to join to {group.Name} group.",
            Label = BasicNotification.LabelType.Information,
            Title = "Join group",
            Url = "/teacher-group/join-request"
        };

        await _notificationService.SendNotificationToUser(group.CreatedBy.ToString(), noti, null, cancellationToken);

        return default(DefaultIdType);
    }
}
