using FSH.WebApi.Application.Class.New;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Class;
using System.Linq;

namespace FSH.WebApi.Application.Class.UserClasses;
public class AddUserInClassRequest : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public Guid ClassesId { get; set; }
    public string? StudentCode { get; set; }
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
    private readonly IUserService _userService;
    private readonly IRepository<Classes> _classesRepository;
    private readonly IUserClassesRepository _userClassesRepository;
    private readonly IStringLocalizer _stringLocalizer;

    public AddUserInClassRequestHandler(IUserClassesRepository userClassesRepository, IUserService userService,
                                        IStringLocalizer<AddUserInClassRequestHandler> localizer, IRepository<Classes> classesRepository) =>
        (_userClassesRepository,_classesRepository, _userService, _stringLocalizer) = (userClassesRepository,classesRepository ,userService, localizer);

    public async Task<Guid> Handle(AddUserInClassRequest request, CancellationToken cancellationToken)
    {

        string userIdString = request.UserId.ToString();

        var user = await _userService.GetAsync(userIdString, cancellationToken);
        var classes = await _classesRepository.GetByIdAsync(request.ClassesId);

        if (user == null)
        {
            throw new NotFoundException(_stringLocalizer["User {0} Not Found.", request.UserId]);
        }

        var getRoleUser = await _userService.GetRolesAsync(userIdString, cancellationToken);
        var checkRole = getRoleUser.Any(x => x.RoleName.Equals("Teacher") && x.Enabled.Equals(true));
        if (checkRole)
        {
            throw new InvalidOperationException(_stringLocalizer["User with role 'teacher' cannot be added to the class."]);
        }


        classes.AddUserToClass(new UserClass
        {
                ClassesId = request.ClassesId,
                UserId = request.UserId,
                IsGender = user.Gender,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                StudentCode = request.StudentCode

        });

        await _classesRepository.UpdateAsync(classes);

        return default(Guid);

    }
}
