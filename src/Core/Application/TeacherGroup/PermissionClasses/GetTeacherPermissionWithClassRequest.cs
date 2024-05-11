using FSH.WebApi.Application.TeacherGroup.TeacherTeams;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams.Specs;
using FSH.WebApi.Domain.TeacherGroup;


namespace FSH.WebApi.Application.TeacherGroup.PermissionClasses;
public class GetTeacherPermissionWithClassRequest : IRequest<TeacherTeamDto>
{
    public Guid TeacherTeamId { get; set; }
    public GetTeacherPermissionWithClassRequest(Guid id)
    {
        TeacherTeamId= id;
    }
}



public class GetTeacherPermissionWithClassRequestHandler : IRequestHandler<GetTeacherPermissionWithClassRequest, TeacherTeamDto>
{
    private readonly IRepository<TeacherTeam> _teacherTeamRepo;
    private readonly IStringLocalizer _t;

    public GetTeacherPermissionWithClassRequestHandler(
        IRepository<TeacherTeam> teacherTeamRepo,
        IStringLocalizer<GetTeacherPermissionWithClassRequestHandler> t)
    {
        _teacherTeamRepo = teacherTeamRepo;
        _t = t;
    }

    public async Task<TeacherTeamDto> Handle(GetTeacherPermissionWithClassRequest request, CancellationToken cancellationToken)
    {
        var teacherTeam = await _teacherTeamRepo.FirstOrDefaultAsync(
            (ISpecification<TeacherTeam, TeacherTeamDto>) new TeacherTeamByIdSpec(request.TeacherTeamId), cancellationToken);

        if (teacherTeam is null)
        {
            throw new NotFoundException(_t["GroupTeacher{0} Not Found.", request.TeacherTeamId]);
        }

        return teacherTeam;
    }
}
