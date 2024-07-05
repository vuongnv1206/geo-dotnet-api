
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class GroupTeacherByUserSpec : Specification<GroupTeacher>
{
    public GroupTeacherByUserSpec(Guid currentUserId)
    {
        Query
            .Include(x => x.TeacherInGroups).ThenInclude(x => x.TeacherTeam)
            .Where(x => x.TeacherInGroups.Any(t => t.TeacherTeam.TeacherId == currentUserId));
    }
}
