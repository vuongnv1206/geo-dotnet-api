using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Domain.TeacherGroup;
namespace FSH.WebApi.Application.TeacherGroup.TeacherTeams;
public class TeacherTeamBySearchSpec : EntitiesByPaginationFilterSpec<TeacherTeam, TeacherTeamDto>
{
    public TeacherTeamBySearchSpec(SearchTeacherTeamsRequest request,Guid currentUserId) : base(request)
    {
        Query.OrderBy(c => c.CreatedOn, !request.HasOrderBy())
            .Where(x => x.CreatedBy == currentUserId);
    }
}
