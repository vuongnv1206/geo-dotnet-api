using FSH.WebApi.Application.Assignments.Dtos;
using FSH.WebApi.Domain.Assignment;

namespace FSH.WebApi.Application.Assignments;

public class AssignmentByIdWithSubjectSpec : Specification<Assignment, AssignmentDetailsDto>, ISingleResultSpecification
{
    public AssignmentByIdWithSubjectSpec(Guid id) =>
        Query
            .Where(p => p.Id == id)
            .Include(p => p.Subject);
}