using FSH.WebApi.Domain.TeacherGroup;


namespace FSH.WebApi.Application.TeacherGroup.TeacherTeams.Specs;
public class TeacherTeamByIdSpec : Specification<TeacherTeam, TeacherTeamDto>, ISingleResultSpecification
{
    public TeacherTeamByIdSpec(Guid id)
    {
        Query.Where(x => x.Id == id);
    }
}
