using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class;
public class ClassesByIdSpec : Specification<Classes>, ISingleResultSpecification
{
    public ClassesByIdSpec(Guid id)
    {
        Query
            .Include(x => x.AssignmentClasses)
            .ThenInclude(a => a.Assignment)
            .Include(x => x.UserClasses).ThenInclude(x => x.Student)
            .Where(x => x.Id == id);
    }
}
