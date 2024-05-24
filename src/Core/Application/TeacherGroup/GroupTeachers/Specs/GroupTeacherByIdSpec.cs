using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class GroupTeacherByIdSpec : Specification<GroupTeacher>,ISingleResultSpecification
{
    public GroupTeacherByIdSpec(Guid id)
    {
        Query
            .Include(x => x.TeacherInGroups)
                .ThenInclude(tig => tig.TeacherTeam)
            .Include(x => x.GroupPermissionInClasses)
            .Where(x => x.Id == id);
    }
}
