using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.TeacherTeams;
public class RemoveTeacherInTeamRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public RemoveTeacherInTeamRequest(Guid id)
    {
        Id = id;
    }
}

public class RemoveTeacherInTeamRequestHandler : IRequestHandler<RemoveTeacherInTeamRequest, Guid>
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

    public async Task<Guid> Handle(RemoveTeacherInTeamRequest request, CancellationToken cancellationToken)
    {
        var teacherInTeam = await _teacherTeamRepo.GetByIdAsync(request.Id, cancellationToken);

        _ = teacherInTeam ?? throw new NotFoundException(_t["Teacher in team {0} Not Found."]);

        await _teacherTeamRepo.DeleteAsync(teacherInTeam, cancellationToken);

        return request.Id;
    }
}
