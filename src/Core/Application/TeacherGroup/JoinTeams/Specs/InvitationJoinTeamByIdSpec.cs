using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinTeams;
public class InvitationJoinTeamByIdSpec : Specification<InviteJoinTeacherTeam>, ISingleResultSpecification
{
    public InvitationJoinTeamByIdSpec(Guid id)
    {
        Query.Where(x => x.Id == id);
    }
}
