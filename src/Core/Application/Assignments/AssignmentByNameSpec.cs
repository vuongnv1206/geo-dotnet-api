using FSH.WebApi.Domain.Assignment;
namespace FSH.WebApi.Application.Assignments;
public class AssignmentByNameSpec : Specification<Assignment>, ISingleResultSpecification
{
    public AssignmentByNameSpec(string name) =>
        Query.Where(p => p.Name == name);
}
