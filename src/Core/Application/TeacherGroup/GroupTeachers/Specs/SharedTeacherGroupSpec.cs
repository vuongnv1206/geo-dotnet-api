using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class SharedTeacherGroupSpec : EntitiesByPaginationFilterSpec<GroupTeacher>
{
    public SharedTeacherGroupSpec(SearchSharedGroupTeachersRequest request, Guid userId)
        : base(request)
    {
        Query
            .Include(x => x.TeacherInGroups)
            .ThenInclude(x => x.TeacherTeam)
            .Where(x => x.TeacherInGroups.Any(t => t.TeacherTeam.TeacherId == userId));
    }
}
