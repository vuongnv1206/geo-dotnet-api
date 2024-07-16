using FSH.WebApi.Application.Class.UserStudents.Dto;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.UserStudents.Spec;
public class StudentBySearchSpec : EntitiesByPaginationFilterSpec<Student, UserStudentDto>
{
    public StudentBySearchSpec(SearchStudentRequest request, Guid currentUserId)
        : base(request)
    {
        Query.OrderBy(c => c.CreatedOn, !request.HasOrderBy())
            .Where(x => x.CreatedBy == currentUserId);
    }
}
