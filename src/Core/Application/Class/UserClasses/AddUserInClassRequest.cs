using FSH.WebApi.Application.Class.New;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.UserClasses;
public class AddUserInClassRequest : IRequest<Guid>
{
    public Guid UserId { get; set;}
    public Guid ClassesId { get; set;}
    public bool IsGender { get;  set; }
    public string StudentCode { get;  set; }
    public string Email { get;  set; }
    public string PhoneNumber { get;  set; }
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
    private readonly IUserClassesRepository _userClassesRepository;
    private readonly IStringLocalizer _stringLocalizer;

    public AddUserInClassRequestHandler(IUserClassesRepository userClassesRepository, IUserService userService,
                                        IStringLocalizer<AddUserInClassRequestHandler> localizer) =>
        (_userClassesRepository,_userService, _stringLocalizer) = (userClassesRepository,userService, localizer);

    public async Task<Guid> Handle(AddUserInClassRequest request, CancellationToken cancellationToken)
    {
        var user = _userService.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(_stringLocalizer["User {0} Not Found.", request.UserId]);
        }

        await _userClassesRepository.AddNewUserInClass(new UserClass(request.ClassesId, request.UserId, request.IsGender, request.StudentCode, request.Email, request.PhoneNumber));

        return default(Guid);

    }
}
