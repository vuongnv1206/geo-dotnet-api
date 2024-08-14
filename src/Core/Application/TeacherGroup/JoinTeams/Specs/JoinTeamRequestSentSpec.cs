using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinTeams;
public class JoinTeamRequestSentSpec : EntitiesByPaginationFilterSpec<JoinTeacherTeamRequest>
{
    public JoinTeamRequestSentSpec(SearchJoinTeacherTeamRequest request, Guid userId)
        : base(request)
    {
        Query.Where(x => x.CreatedBy == userId)
          .OrderBy(x => x.Status == JoinTeacherGroupStatus.Pending ? 0 : 1)
          .ThenByDescending(x => x.CreatedOn);
    }
}
