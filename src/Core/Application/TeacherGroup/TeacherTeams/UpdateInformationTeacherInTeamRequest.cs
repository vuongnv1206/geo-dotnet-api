using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.TeacherTeams;
public class UpdateInformationTeacherInTeamRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string TeacherName { get; set; } = null!;
    public string Contact { get; set; } = null!;
}

public class UpdateInformationTeacherInTeamRequestHandler : IRequestHandler<UpdateInformationTeacherInTeamRequest, Guid>
{
    private readonly IRepositoryWithEvents<TeacherTeam> _teacherTeamRepo;
    private readonly IStringLocalizer _t;
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;

    public UpdateInformationTeacherInTeamRequestHandler(
        IRepositoryWithEvents<TeacherTeam> teacherTeamRepo,
        IStringLocalizer<UpdateInformationTeacherInTeamRequestHandler> t,
        IUserService userService,
        ICurrentUser currentUser)
    {
        _teacherTeamRepo = teacherTeamRepo;
        _t = t;
        _userService = userService;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(UpdateInformationTeacherInTeamRequest request, CancellationToken cancellationToken)
    {
        var teacherTeam = await _teacherTeamRepo.GetByIdAsync(request.Id);
        if (teacherTeam is null)
            throw new NotFoundException(_t["teacher {0} Not Found.", request.Id]);

        var teachersUseContact = await _teacherTeamRepo.ListAsync(
         new TeacherTeamByContactSpec(request.Contact, _currentUser.GetUserId()), cancellationToken);

        if (teachersUseContact.Any(x => x.Id != teacherTeam.Id))
        {
            throw new ConflictException(_t["Teacher's contact exist in team"]);
        }

        switch (request.Contact.CheckType())
        {
            case ValidationType.EmailAddress:
                var teacher = await _userService.GetUserDetailByEmailAsync(request.Contact, cancellationToken);
                teacherTeam.TeacherId = teacher?.Id ?? (Guid?)null;

                teacherTeam.Update(request.TeacherName, request.Contact, string.Empty);
                break;
            case ValidationType.PhoneNumber:
                teacher = await _userService.GetUserDetailByPhoneAsync(request.Contact, cancellationToken);
                teacherTeam.TeacherId = teacher?.Id ?? (Guid?)null;

                teacherTeam.Update(request.TeacherName, string.Empty, request.Contact);
                break;
            default:
                throw new ConflictException(_t["This Contact is invalid!"]);
        }

        await _teacherTeamRepo.UpdateAsync(teacherTeam, cancellationToken);
        return request.Id;
    }
}
