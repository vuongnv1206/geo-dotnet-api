using FSH.WebApi.Application.TeacherGroup.GroupTeachers.Specs;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.TeacherTeams;
public class RemoveTeacherInTeamRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
}

public class RemoveTeacherInTeamRequestHandler : IRequestHandler<RemoveTeacherInTeamRequest, DefaultIdType>
{
    private readonly IRepositoryWithEvents<TeacherTeam> _teacherTeamRepo;
    private readonly IStringLocalizer _t;

    public RemoveTeacherInTeamRequestHandler(
        IRepositoryWithEvents<TeacherTeam> teacherTeamRepo,
        IStringLocalizer<RemoveTeacherInTeamRequestHandler> t)
    {
        _teacherTeamRepo = teacherTeamRepo;
        _t = t;
    }

    public async Task<DefaultIdType> Handle(RemoveTeacherInTeamRequest request, CancellationToken cancellationToken)
    {
        var teacherInTeam = await _teacherTeamRepo.GetByIdAsync(request.Id, cancellationToken);

        _ = teacherInTeam ?? throw new NotFoundException(_t["Teacher in team {0} Not Found."]);

        await _teacherTeamRepo.DeleteAsync(teacherInTeam, cancellationToken);

        return request.Id;
    }
}
