using FSH.WebApi.Application.Identity.Users;
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
    private readonly ITeacherInGroupRepository _repository;
    private readonly IRepository<GroupTeacher> _groupTeacherRepository;
    private readonly IUserService _userService;
    private readonly IStringLocalizer _t;

    public RemoveTeacherInGroupRequestHandler(
        ITeacherInGroupRepository repository,
        IStringLocalizer<RemoveTeacherInGroupRequestHandler> t,
        IRepository<GroupTeacher> groupTeacherRepository
        )
    {
        _repository = repository;
        _t = t;
        _groupTeacherRepository = groupTeacherRepository;
    }

    public async Task<DefaultIdType> Handle(RemoveTeacherInGroupRequest request, CancellationToken cancellationToken)
    {
        var teacherInGroup = await _repository.GetTeacherInGroup(new TeacherInGroup
        {
            TeacherId = request.TeacherId,
            GroupTeacherId = request.GroupId,
        });
        if (teacherInGroup is null)
            throw new NotFoundException(_t["Teacher or Group {0} Not Found."]);

        await _repository.DeleteTeacherInGroupAsync(teacherInGroup);

        return default(DefaultIdType);
    }
}
