using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class GroupTeachersBySearchSpec : EntitiesByPaginationFilterSpec<GroupTeacher, GroupTeacherDto>
{
    public GroupTeachersBySearchSpec(SearchGroupTeachersRequest request) : base(request)
    {
        Query.OrderBy(c => c.Name, !request.HasOrderBy());
    }
}
