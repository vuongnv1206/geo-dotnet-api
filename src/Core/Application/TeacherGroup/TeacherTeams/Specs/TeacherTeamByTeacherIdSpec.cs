using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.TeacherTeams;
public class TeacherTeamByTeacherIdSpec : Specification<TeacherTeam>, ISingleResultSpecification
{
    public TeacherTeamByTeacherIdSpec(Guid teacherId, Guid adminId)
    {
        Query.Where(x => x.TeacherId == teacherId && x.CreatedBy == adminId);
    }
}
