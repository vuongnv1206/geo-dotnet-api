using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.TeacherTeams;
public class TeacherTeamByContactSpec : Specification<TeacherTeam>
{
    public TeacherTeamByContactSpec(string contact, Guid userId)
    {
        Query.Where(x => (x.Phone.Trim().ToLower().Equals(contact.Trim().ToLower()) || x.Email.Trim().ToLower().Equals(contact.Trim().ToLower()))
                        && x.CreatedBy == userId);
    }
}
