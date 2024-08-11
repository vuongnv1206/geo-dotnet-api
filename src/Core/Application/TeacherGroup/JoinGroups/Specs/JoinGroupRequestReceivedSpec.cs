using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinGroups;
public class JoinGroupRequestReceivedSpec : EntitiesByPaginationFilterSpec<JoinGroupTeacherRequest>
{
    public JoinGroupRequestReceivedSpec(SearchJoinGroupTeacherRequest request, Guid userId)
        : base(request)
    {
        Query.Where(x => x.ReceiverId == userId)
            .OrderBy(x => x.Status == JoinTeacherGroupStatus.Pending ? 0 : 1)
            .ThenByDescending(x => x.CreatedOn)
            .Include(x => x.GroupTeacher)
            .Include(x => x.TeacherTeam);
    }
}
