using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.UserClasses;
public class DeleteUserInClassRequest : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public Guid ClassesId { get; set; }
}

public class DeleteUserInClassRequestValidator : CustomValidator<DeleteUserInClassRequest>
{
    public DeleteUserInClassRequestValidator(IReadRepository<Classes> classRepos, IStringLocalizer<DeleteUserInClassRequestValidator> T)
    {
        RuleFor(p => p.ClassesId)
            .NotEmpty()
            .MustAsync(async (classId, ct) => await classRepos.GetByIdAsync(classId, ct) is not null)
            .WithMessage((_, classId) => T["Classes {0} Not Found.", classId]);
    }
}

public class DeleteUserInClassRequestHandler : IRequestHandler<DeleteUserInClassRequest, Guid>
{
    private readonly IUserClassesRepository _userClassesRepository;
    private readonly IStringLocalizer _stringLocalizer;
    private readonly IUserService _userService;

    public DeleteUserInClassRequestHandler(IUserClassesRepository userClassesRepository, IUserService userService,
                                           IStringLocalizer<DeleteUserInClassRequestHandler> stringLocalizer) =>
        (_userClassesRepository, _userService, _stringLocalizer) = (userClassesRepository, userService, stringLocalizer);
    public async Task<Guid> Handle(DeleteUserInClassRequest request, CancellationToken cancellationToken)
    {

        var user = _userService.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(_stringLocalizer["User {0} Not Found.", request.UserId]);
        }

        await _userClassesRepository.DeleteUserInClass(request.UserId, request.ClassesId);

        return default(Guid);
    }
}
