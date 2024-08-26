using FSH.WebApi.Domain.Class;


namespace FSH.WebApi.Application.Class;
public class ClassToDeleteSpec : Specification<Classes>
{
    public ClassToDeleteSpec(Guid classId)
    {
        Query.Include(x => x.AssignmentClasses)
            .Include(x => x.UserClasses)
            .Include(x => x.Posts).ThenInclude(x => x.PostLikes)
            .Include(x => x.Posts).ThenInclude(x => x.Comments)
            .Include(c => c.TeacherPermissionInClasses)
            .Include(c => c.GroupPermissionInClasses)
            .Include(c => c.UserClasses)
            ;
    }
}
