using FSH.WebApi.Domain.Assignment;

namespace FSH.WebApi.Application.Assignments;
public class AssignmentsBySubjectSpec : Specification<Assignment>
{
    public AssignmentsBySubjectSpec(Guid subjectId) =>
        Query.Where(p => p.SubjectId == subjectId);
}
