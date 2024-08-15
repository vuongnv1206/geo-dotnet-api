using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinTeams;
public class JoinTeacherTeamByIdSpec : Specification<JoinTeacherTeamRequest>, ISingleResultSpecification
{
    public JoinTeacherTeamByIdSpec(Guid id)
    {
        Query.Where(x => x.Id == id);
    }
}
