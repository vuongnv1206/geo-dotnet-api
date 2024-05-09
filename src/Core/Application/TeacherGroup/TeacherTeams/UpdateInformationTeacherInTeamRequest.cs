using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.TeacherGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public UpdateInformationTeacherInTeamRequestHandler(
        IRepositoryWithEvents<TeacherTeam> teacherTeamRepo,
        IStringLocalizer<UpdateInformationTeacherInTeamRequestHandler> t)
    {
        _teacherTeamRepo = teacherTeamRepo;
        _t = t;
    }

    public async Task<Guid> Handle(UpdateInformationTeacherInTeamRequest request, CancellationToken cancellationToken)
    {
        var teacherTeam = await _teacherTeamRepo.GetByIdAsync(request.Id);
        if (teacherTeam is null)
            throw new NotFoundException(_t["teacher {0} Not Found.", request.Id]);

        switch (request.Contact.CheckType())
        {
            case ValidationType.EmailAddress:
                teacherTeam.Update(request.TeacherName, request.Contact, string.Empty);
                break;
            case ValidationType.PhoneNumber:
                teacherTeam.Update(request.TeacherName, string.Empty, request.Contact);
                break;
            default:
                throw new ConflictException(_t["This Contact is invalid!"]);
        }
        await _teacherTeamRepo.UpdateAsync(teacherTeam, cancellationToken);
        return request.Id;
    }
}
