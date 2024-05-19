using FSH.WebApi.Domain.TeacherGroup;


namespace FSH.WebApi.Application.TeacherGroup.TeacherTeams.Specs;
public class TeacherTeamByIdSpec : Specification<TeacherTeam, TeacherTeamDto>, ISingleResultSpecification
{
    public TeacherTeamByIdSpec(Guid id, Guid userId)
    {
        Query
            .Where(x => x.Id == id && x.CreatedBy == userId)
            .Include(x => x.TeacherPermissionInClasses);
    }
}
