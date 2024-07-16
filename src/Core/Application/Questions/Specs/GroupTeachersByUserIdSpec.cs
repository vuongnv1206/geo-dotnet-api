using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.Questions.Specs;
internal class GroupTeachersByUserIdSpec : Specification<GroupTeacher>, ISingleResultSpecification
{
    public GroupTeachersByUserIdSpec(DefaultIdType userId)
    {
        Query
        .Include(x => x.TeacherInGroups)
        .ThenInclude(tt => tt.TeacherTeam)
        .Where(x => x.TeacherInGroups.Any(x => x.TeacherTeam.TeacherId == userId));
    }
}