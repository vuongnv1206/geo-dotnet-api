using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class GroupTeachersBySearchSpec : EntitiesByPaginationFilterSpec<GroupTeacher, GroupTeacherDto>
{
    public GroupTeachersBySearchSpec(SearchGroupTeachersRequest request, Guid currentUserId)
        : base(request)
    {
        Query.OrderBy(c => c.CreatedOn, !request.HasOrderBy())
               .Where(x => x.CreatedBy == currentUserId);
    }
}
