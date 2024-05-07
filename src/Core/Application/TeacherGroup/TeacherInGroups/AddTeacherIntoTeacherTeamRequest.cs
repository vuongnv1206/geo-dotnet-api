using FSH.WebApi.Application.Common.Validation;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Domain.TeacherGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.TeacherGroup.TeacherInGroups;
public class AddTeacherIntoTeacherTeamRequest : IRequest<Guid>
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

public class AddTeacherIntoTeacherTeamRequestHandler : IRequestHandler<AddTeacherIntoTeacherTeamRequest, Guid>
{
    private readonly IRepositoryWithEvents<TeacherTeam> _repository;
    private readonly IStringLocalizer _t;
    private readonly IUserService _userService;

    public AddTeacherIntoTeacherTeamRequestHandler(
        IRepositoryWithEvents<TeacherTeam> repository,
        IStringLocalizer<AddTeacherIntoTeacherTeamRequestHandler> t,
        IUserService userService)
    {
        _repository = repository;
        _t = t;
        _userService = userService;
    }

    public async Task<DefaultIdType> Handle(AddTeacherIntoTeacherTeamRequest request, CancellationToken cancellationToken)
    {
        var teacherTeam = new TeacherTeam()
        {
            TeacherName = request.TeacherName,
        };

        switch(request.Contact.CheckType())
        {
            case ValidationType.EmailAddress:
                teacherTeam.Email = request.Contact;
                var teacher = await _userService.GetUserDetailByEmailAsync(request.Contact, cancellationToken);
                if (teacher is not null)
                    teacherTeam.TeacherId = teacher.Id;
                break;
            case ValidationType.PhoneNumber:
                teacherTeam.Phone = request.Contact;
                teacher = await _userService.GetUserDetailByPhoneAsync(request.Contact, cancellationToken);
                if (teacher is not null)
                    teacherTeam.TeacherId = teacher.Id;
                break;
            default:
                throw new ConflictException(_t["This Contact is invalid!"]);
        }

        await _repository.AddAsync(teacherTeam);
        return teacherTeam.Id;
    }
}