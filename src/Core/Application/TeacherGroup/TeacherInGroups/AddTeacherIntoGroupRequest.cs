using Ardalis.Specification;
using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.TeacherInGroups;
public class AddTeacherIntoGroupRequest : IRequest<Guid>
{
    public Guid GroupId { get; set; }
    public string EmailTeacher { get; set; } = null!;
}

public class AddTeacherIntoTeamRequestvalidator : CustomValidator<AddTeacherIntoGroupRequest>
{
    public AddTeacherIntoTeamRequestvalidator()
    {

    }
}

public class AddTeacherIntoTeamRequestHandler : IRequestHandler<AddTeacherIntoGroupRequest, Guid>
{
    private readonly ITeacherInGroupRepository _repository;
    private readonly IRepository<GroupTeacher> _groupTeacherRepository;
    private readonly IUserService _userService;
    private readonly IStringLocalizer _t;


    public AddTeacherIntoTeamRequestHandler(
        ITeacherInGroupRepository repository,
        IRepository<GroupTeacher> groupTeacherRepository,
        IUserService userService,
        IStringLocalizer<AddTeacherIntoTeamRequestHandler> t)
    => (_repository, _groupTeacherRepository, _userService, _t)
        = (repository, groupTeacherRepository, userService, t);

    public async Task<DefaultIdType> Handle(AddTeacherIntoGroupRequest request, CancellationToken cancellationToken)
    {
        bool existGroup = await _groupTeacherRepository
           .AnyAsync(new GroupTeacherByIdSpec(request.GroupId));
        if (!existGroup)
        {
            throw new NotFoundException(_t["Brand {0} Not Found."]);
        }

        //note: need to check teacher in my team
        var teacherToAdd = await _userService.GetUserDetailByEmailAsync(request.EmailTeacher, cancellationToken);
        if (teacherToAdd.Id == Guid.Empty)
        {
            // create new user with active false
        }
        else
        {
            await _repository.AddTeacherIntoGroup(new TeacherInGroup
            {
                TeacherId = Guid.NewGuid(),
                GroupTeacherId = request.GroupId
            });
        }

        return default(DefaultIdType);
    }
}
