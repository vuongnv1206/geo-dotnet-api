using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinGroups;
public class JoinGroupRequestSentSpec : EntitiesByPaginationFilterSpec<JoinGroupTeacherRequest>
{
    public JoinGroupRequestSentSpec(SearchJoinGroupTeacherRequest request, Guid userId)
       : base(request)
    {
        Query.Where(x => x.CreatedBy == userId)
            .OrderBy(x => x.CreatedOn)
            .Include(x => x.GroupTeacher)
            .Include(x => x.TeacherTeam);
    }
}
