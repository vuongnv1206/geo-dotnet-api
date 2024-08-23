using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.TeacherTeams;
public class AddTeacherIntoTeacherTeamRequest : IRequest<DefaultIdType>
{
    public string TeacherName { get; set; } = null!;
    public string Contact { get; set; } = null!;
}

public class AddTeacherIntoTeacherTeamRequestValidator : CustomValidator<AddTeacherIntoTeacherTeamRequest>
{
    public AddTeacherIntoTeacherTeamRequestValidator(IReadRepository<TeacherTeam> repository, IStringLocalizer<AddTeacherIntoTeacherTeamRequestValidator> T) =>
       RuleFor(p => p.TeacherName)
           .NotEmpty()
           .MaximumLength(50);
}

public class AddTeacherIntoTeacherTeamRequestHandler : IRequestHandler<AddTeacherIntoTeacherTeamRequest, DefaultIdType>
{
    private readonly IRepositoryWithEvents<TeacherTeam> _repository;
    private readonly IStringLocalizer _t;
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;

    public AddTeacherIntoTeacherTeamRequestHandler(
        IRepositoryWithEvents<TeacherTeam> repository,
        IStringLocalizer<AddTeacherIntoTeacherTeamRequestHandler> t,
        IUserService userService,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _t = t;
        _userService = userService;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType> Handle(AddTeacherIntoTeacherTeamRequest request, CancellationToken cancellationToken)
    {
        var teacherTeam = new TeacherTeam()
        {
            TeacherName = request.TeacherName,
        };

        var existDuplicateContact = await _repository.AnyAsync(
            new TeacherTeamByContactSpec(request.Contact, _currentUser.GetUserId()), cancellationToken);
        if (existDuplicateContact)
        {
            throw new ConflictException(_t["Teacher's contact exist in team"]);
        }

        switch (request.Contact.CheckType())
        {
            case ValidationType.EmailAddress:
                teacherTeam.Email = request.Contact;
                var teacher = await _userService.GetUserDetailByEmailAsync(request.Contact, cancellationToken);
                teacherTeam.TeacherId = teacher?.Id ?? (Guid?)null;
                break;
            case ValidationType.PhoneNumber:
                teacherTeam.Phone = request.Contact;
                teacher = await _userService.GetUserDetailByPhoneAsync(request.Contact, cancellationToken);
                teacherTeam.TeacherId = teacher?.Id ?? (Guid?)null;
                break;
            default:
                throw new ConflictException(_t["This Contact is invalid!"]);
        }

        await _repository.AddAsync(teacherTeam);
        return teacherTeam.Id;
    }
}