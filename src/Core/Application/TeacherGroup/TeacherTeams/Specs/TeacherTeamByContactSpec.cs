using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.TeacherTeams;
public class TeacherTeamByContactSpec : Specification<TeacherTeam>
{
    public TeacherTeamByContactSpec(string contact, Guid userId)
    {
        Query.Where(x => (x.Phone.Equals(contact) || x.Email.Equals(contact))
                        && x.CreatedBy == userId);
    }
}
