using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers.Specs;
public class TeacherTeamByIdSpec : Specification<TeacherTeam>, ISingleResultSpecification
{
    public TeacherTeamByIdSpec(Guid id)
    {
        Query.Where(x => x.Id == id);
    }
}
