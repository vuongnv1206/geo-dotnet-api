using FSH.WebApi.Application.Assignments.Dtos;
using FSH.WebApi.Domain.Assignment;

namespace FSH.WebApi.Application.Assignments;

public class AssignmentByIdWithSubjectSpec : Specification<Assignment, AssignmentDetailsDto>, ISingleResultSpecification
{
    public AssignmentByIdWithSubjectSpec(Guid id) =>
        Query
           .Include(a => a.AssignmentClasses).ThenInclude(a => a.Classes)
           .Include(a => a.Subject)
           .Where(a => a.Id == id);
}