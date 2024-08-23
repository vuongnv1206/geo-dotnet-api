using FSH.WebApi.Application.Common.Specification;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Examination.PaperAccesses;
public class ClassHasStudentAccessPaperSpec : EntitiesByPaginationFilterSpec<Classes>
{
    public ClassHasStudentAccessPaperSpec(GetGetAssigneesInPaperRequest request, List<Guid> studentIds, Guid userId)
        : base (request)
    {
        Query.Where(x => x.CreatedBy == userId)
            .Include(x => x.GroupClass)
            .Include(x => x.UserClasses)
            .ThenInclude(x => x.Student)
            .Where(x => !request.GroupClassId.HasValue || x.GroupClassId == request.GroupClassId)
            .Where(x => !request.ClassId.HasValue || x.Id == request.ClassId)
            .Where(x => x.UserClasses.Any(uc => studentIds.Contains(uc.StudentId)));
    }
}
