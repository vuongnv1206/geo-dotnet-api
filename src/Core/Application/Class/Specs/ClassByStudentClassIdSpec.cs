using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.Specs;
public class ClassByStudentClassIdSpec : Specification<Classes>, ISingleResultSpecification
{
    public ClassByStudentClassIdSpec(Guid studentId)
    {
        Query
           .Include(c => c.UserClasses)
               .ThenInclude(uc => uc.Student)
           .Where(c => c.UserClasses.Any(uc => uc.Student.Id == studentId))
           .Include(c => c.PaperAccesses);
    }
}
