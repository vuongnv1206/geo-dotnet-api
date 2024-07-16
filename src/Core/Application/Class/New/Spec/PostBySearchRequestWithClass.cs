using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.New.Spec;
public class PostBySearchRequestWithClass : Specification<Post>
{
    public PostBySearchRequestWithClass(Guid? classId)
    {
        Query
            .Include(p => p.Classes)
            .Where(p => p.ClassesId.Equals(classId));
    }
}
