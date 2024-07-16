using FSH.WebApi.Domain.Assignment;

namespace FSH.WebApi.Application.Assignments;
public class AssignmentByIdSpec : Specification<Assignment>, ISingleResultSpecification
{
    public AssignmentByIdSpec(Guid id)
    {
        Query
            .Include(a => a.AssignmentClasses)
            .Include(a => a.AssignmentStudents).ThenInclude(x => x.Student)
            .Where(x => x.Id == id);
    }
}
