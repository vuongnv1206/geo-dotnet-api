using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.Specs;
public class ClassesByStudentIdSpec : Specification<Classes>, ISingleResultSpecification
{
    public ClassesByStudentIdSpec(Guid userId)
    {
        Query
            .Include(c => c.UserClasses)
                .ThenInclude(uc => uc.Student)
            .Where(c => c.UserClasses.Any(uc => uc.Student.StId == userId))
            .Include(c => c.PaperAccesses);
    }
}
