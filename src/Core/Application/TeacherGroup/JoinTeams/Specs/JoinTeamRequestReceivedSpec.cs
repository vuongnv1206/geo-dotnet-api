using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinTeams;
public class JoinTeamRequestReceivedSpec : EntitiesByPaginationFilterSpec<JoinTeacherTeamRequest>
{
    public JoinTeamRequestReceivedSpec(SearchJoinTeacherTeamRequest request, Guid userId)
        : base(request)
    {
        Query.Where(x => x.AdminTeamId == userId)
            .OrderBy(x => x.Status == JoinTeacherGroupStatus.Pending ? 0 : 1)
            .ThenByDescending(x => x.CreatedOn);
    }
}
