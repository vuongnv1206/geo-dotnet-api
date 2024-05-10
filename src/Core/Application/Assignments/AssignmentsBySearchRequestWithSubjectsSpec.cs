using FSH.WebApi.Application.Assignments.Dtos;
using FSH.WebApi.Domain.Assignment;

namespace FSH.WebApi.Application.Assignments;
public class AssignmentsBySearchRequestWithSubjectsSpec : EntitiesByPaginationFilterSpec<Assignment, AssignmentDto>
{
    public AssignmentsBySearchRequestWithSubjectsSpec(SearchAssignmentsRequest request)
        : base(request) =>
        Query
            .Include(p => p.Subject)
            .OrderBy(c => c.Name, !request.HasOrderBy())
            .Where(p => p.SubjectId.Equals(request.SubjectId!.Value), request.SubjectId.HasValue);
}
