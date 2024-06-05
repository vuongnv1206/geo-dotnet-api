using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.UserClasses;
public class UpdateUserInClassRequest : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public Guid ClassesId { get; set; }
    public string? StudentCode { get; set; }
    public bool IsGender { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}

public class UpdateUserInClassRequestValidator : CustomValidator<UpdateUserInClassRequest>
{
    public UpdateUserInClassRequestValidator(IRepository<Classes> repository, IUserService userService, IStringLocalizer<UpdateUserInClassRequestValidator> T)
    {
        RuleFor(g => g.ClassesId)
            .NotEmpty()
            .MustAsync(async (classesId, ct) => await repository.FirstOrDefaultAsync(new ClassByIdWithGroupClassSpec(classesId), ct) is not null)
            .WithMessage((_, classesId) => T["Classes {0} not found.", classesId]);
    }
}

public class UpdateUserInClassRequestHandler : IRequestHandler<UpdateUserInClassRequest, Guid>
{
    private readonly IUserClassesRepository _userClassesRepository;

    public UpdateUserInClassRequestHandler(IUserClassesRepository userClassesRepository)
    {
        _userClassesRepository = userClassesRepository;
    }

    public async Task<Guid> Handle(UpdateUserInClassRequest request, CancellationToken cancellationToken)
    {
        var userClasses = await _userClassesRepository.GetUserDetailInClasses(request.UserId, request.ClassesId);

        if (userClasses == null)
        {
            throw new NotFoundException("UserClasses not found for the given UserId and ClassesId.");
        }

        var data = userClasses.Update(request.IsGender, request.StudentCode, request.Email, request.PhoneNumber);

        await _userClassesRepository.UpdateUserInClasses(data);
        return default;
    }
}

