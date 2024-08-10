using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinGroups;
public class ExistTeacherMemberInTeamSpec : Specification<TeacherTeam>, ISingleResultSpecification
{
    public ExistTeacherMemberInTeamSpec(Guid adminId, Guid memberId)
    {
        Query.Where(x => x.CreatedBy == adminId && x.TeacherId == memberId);
    }
}
