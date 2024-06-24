using FSH.WebApi.Application.Class.New;
using FSH.WebApi.Application.Class.UserStudents;
using FSH.WebApi.Application.Class.UserStudents.Spec;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams.Specs;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.Class.UserClasses;
public class AddUserInClassRequest : IRequest<Guid>
{
    public Guid StudentId { get; set; }
    public Guid ClassesId { get; set; }
}

public class AddUserInClassRequestValidator : CustomValidator<AddUserInClassRequest>
{
    public AddUserInClassRequestValidator(IReadRepository<Classes> classRepos, IStringLocalizer<CreateNewsRequestValidator> T)
    {
        RuleFor(p => p.ClassesId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await classRepos.GetByIdAsync(id, ct) is not null)
            .WithMessage((_, id) => T["Classes {0} Not Found.", id]);
    }
}

public class AddUserInClassRequestHandler : IRequestHandler<AddUserInClassRequest, Guid>
{
    private readonly IRepository<UserStudent> _userStudentRepository;
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer<CreateUserStudentRequestHandler> _t;
    private readonly IRepository<Classes> _classRepository;

    public AddUserInClassRequestHandler(IUserService userService, ICurrentUser currentUser,
                                           IStringLocalizer<CreateUserStudentRequestHandler> t, IRepository<UserStudent> userStudentRepository, IRepository<Classes> classRepository)
    {
        _userService = userService;
        _currentUser = currentUser;
        _t = t;
        _classRepository = classRepository;
        _userStudentRepository = userStudentRepository;
    }

    public async Task<Guid> Handle(AddUserInClassRequest request, CancellationToken cancellationToken)
    {
        var classes = await _classRepository.GetByIdAsync(request.ClassesId);
        if (classes is null)
        {
            throw new NotFoundException(_t["Class {0} Not Found.", request.ClassesId]);
        }

        var userInClass = await _userStudentRepository
      .FirstOrDefaultAsync(new UserStudentByIdSpec(request.StudentId, _currentUser.GetUserId()));

        if (userInClass is null)
        {
            throw new NotFoundException(_t["Student {0} Not Found.", request.StudentId]);
        }
        else
        {
            if (userInClass.StudentId == Guid.Empty)
            {
                // create new user active false
            }

            classes.AddUserInClass(new UserClass
            {
                ClassesId = request.ClassesId,
                UserStudentId = request.StudentId
            });

            await _classRepository.UpdateAsync(classes);
        }

        return default(Guid);
    }
}
