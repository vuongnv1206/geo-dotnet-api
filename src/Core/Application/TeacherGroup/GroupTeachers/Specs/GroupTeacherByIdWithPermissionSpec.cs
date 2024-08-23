using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class GroupTeacherByIdWithPermissionSpec : Specification<GroupTeacher>, ISingleResultSpecification
{
    public GroupTeacherByIdWithPermissionSpec(Guid id, Guid userId)
    {
        Query
            .Include(x => x.TeacherInGroups)
                .ThenInclude(tig => tig.TeacherTeam)
            .Include(x => x.GroupPermissionInClasses)
                .ThenInclude(gpc => gpc.Classroom)
            .Include(x => x.JoinGroupRequests)
            .Where(x => x.Id == id && (x.CreatedBy == userId || x.TeacherInGroups.Any(tig => tig.TeacherTeam.TeacherId == userId)));
    }
}
