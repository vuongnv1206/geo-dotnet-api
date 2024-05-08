using FSH.WebApi.Domain.TeacherGroup;


namespace FSH.WebApi.Application.TeacherGroup.PermissionClasses;
public class TeacherTeamByIdWithPermissionSpec : Specification<TeacherTeam>, ISingleResultSpecification
{
    public TeacherTeamByIdWithPermissionSpec(Guid id)
    {
        Query.Where(x => x.Id == id).Include(x => x.TeacherPermissionInClasses);
    }
}
