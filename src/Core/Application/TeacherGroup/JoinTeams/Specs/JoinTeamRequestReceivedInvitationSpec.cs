using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinTeams;
public class JoinTeamRequestReceivedInvitationSpec : Specification<JoinTeacherTeamRequest>, ISingleResultSpecification
{
    public JoinTeamRequestReceivedInvitationSpec(Guid userId)
    {
        Query.Where(x => x.AdminTeamId == userId
                    && x.Status != JoinTeacherGroupStatus.Cancel
                    && x.InvitationId.HasValue)
           .OrderBy(x => x.Status == JoinTeacherGroupStatus.Pending ? 0 : 1)
           .ThenByDescending(x => x.CreatedOn);
    }
}
