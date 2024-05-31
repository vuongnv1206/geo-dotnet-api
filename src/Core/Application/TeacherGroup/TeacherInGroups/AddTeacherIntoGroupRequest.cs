using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams.Specs;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.TeacherInGroups;
public class AddTeacherIntoGroupRequest : IRequest<Guid>
{
    public Guid GroupId { get; set; }
    public Guid TeacherId { get; set; }
}

public class AddTeacherIntoTeamRequestvalidator : CustomValidator<AddTeacherIntoGroupRequest>
{
    public AddTeacherIntoTeamRequestvalidator()
    {

    }
}

public class AddTeacherIntoTeamRequestHandler : IRequestHandler<AddTeacherIntoGroupRequest, Guid>
{
    private readonly IRepository<GroupTeacher> _groupTeacherRepository;
    private readonly IUserService _userService;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;
    private readonly IReadRepository<TeacherTeam> _teacherTeamRepository;

    public AddTeacherIntoTeamRequestHandler(
        IRepository<GroupTeacher> groupTeacherRepository,
        IUserService userService,
        IStringLocalizer<AddTeacherIntoTeamRequestHandler> t,
        ICurrentUser currentUser,
        IReadRepository<TeacherTeam> teacherTeamRepository)
    => (_groupTeacherRepository, _userService, _t, _currentUser, _teacherTeamRepository)
        = (groupTeacherRepository, userService, t, currentUser, teacherTeamRepository);

    public async Task<DefaultIdType> Handle(AddTeacherIntoGroupRequest request, CancellationToken cancellationToken)
    {
        var group = await _groupTeacherRepository
           .GetByIdAsync(request.GroupId);
        if (group is null)
        {
            throw new NotFoundException(_t["Group {0} Not Found.", request.GroupId]);
        }

        var userId = _currentUser.GetUserId();
        if (!group.CanUpdate(userId))
            throw new ForbiddenException(_t["You not have this permission"]);

        // note: need to check teacher in my team
        var teacherInTeam = await _teacherTeamRepository
            .FirstOrDefaultAsync(new TeacherTeamByIdSpec(request.TeacherId, userId));

        if (teacherInTeam is null)
        {
            throw new NotFoundException(_t["Teacher {0} Not Found.", request.TeacherId]);
        }
        else
        {
            if (teacherInTeam.TeacherId == Guid.Empty)
            {
                // create new user active false
            }

            group.AddTeacherIntoGroup(new TeacherInGroup
            {
                GroupTeacherId = request.GroupId,
                TeacherTeamId = request.TeacherId
            });

            await _groupTeacherRepository.UpdateAsync(group);
        }

        return default(DefaultIdType);
    }
}
