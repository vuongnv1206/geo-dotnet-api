using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.TeacherInGroups;
public class RemoveTeacherInGroupRequest : IRequest<Guid>
{
    public Guid GroupId { get; set; }
    public Guid TeacherId { get; set; }
}

public class RemoveTeacherInGroupRequestValidator : CustomValidator<RemoveTeacherInGroupRequest>
{
    public RemoveTeacherInGroupRequestValidator()
    {

    }
}

public class RemoveTeacherInGroupRequestHandler : IRequestHandler<RemoveTeacherInGroupRequest, Guid>
{
    private readonly IRepository<GroupTeacher> _groupTeacherRepository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;

    public RemoveTeacherInGroupRequestHandler(
        IStringLocalizer<RemoveTeacherInGroupRequestHandler> t,
        IRepository<GroupTeacher> groupTeacherRepository,
        ICurrentUser currentUser)
    {
        _t = t;
        _groupTeacherRepository = groupTeacherRepository;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType> Handle(RemoveTeacherInGroupRequest request, CancellationToken cancellationToken)
    {
        var group = await _groupTeacherRepository.FirstOrDefaultAsync(new GroupTeacherByIdSpec(request.GroupId));
        if (group is null)
            throw new NotFoundException(_t["Group {0} Not Found."]);

        if (!group.CanUpdate(_currentUser.GetUserId()))
            throw new ForbiddenException(_t["You cannot have permissio update with {0}", request.GroupId]);

        if (group.TeacherInGroups.Any())
        {
            var teacherInGroup = group.TeacherInGroups?
                .FirstOrDefault(x => x.TeacherTeamId == request.TeacherId);

            if (teacherInGroup is null)
                throw new NotFoundException(_t["Teacher {0} Not Found.", request.TeacherId]);

            group.RemoveTeacherInGroup(teacherInGroup);

            await _groupTeacherRepository.UpdateAsync(group);
        }

        return default(DefaultIdType);
    }
}
