using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinTeams;
public class ExistJoinRequestTeacherTeamSpec : Specification<JoinTeacherTeamRequest>, ISingleResultSpecification
{
    public ExistJoinRequestTeacherTeamSpec(Guid userId, Guid adminId)
    {
        Query.Where(x => x.CreatedBy == userId && x.AdminTeamId == adminId);
    }
}
