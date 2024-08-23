using FSH.WebApi.Application.Common.Specification;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinTeams;
public class SearchInviteJoinTeamSpec : Specification<InviteJoinTeacherTeam>, ISingleResultSpecification
{
    public SearchInviteJoinTeamSpec(Guid userId)
    {
        Query.Where(x => x.CreatedBy == userId).OrderByDescending(x => x.CreatedBy);
    }
}
