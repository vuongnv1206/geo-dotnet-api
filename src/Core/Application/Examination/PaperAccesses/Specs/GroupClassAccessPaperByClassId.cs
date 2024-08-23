using FSH.WebApi.Application.Common.Specification;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Examination.PaperAccesses;
public class GroupClassAccessPaperByClassId : EntitiesByPaginationFilterSpec<GroupClass>
{
    public GroupClassAccessPaperByClassId(List<Guid> classIds, List<Guid> studentIds, GetGroupClassesAccessPaperRequest request, Guid userId)
        : base(request)
    {
       Query.Include(x => x.Classes)
        .ThenInclude(c => c.UserClasses)
        .ThenInclude(c => c.Student)
        .Where(x => x.CreatedBy == userId &&  x.Classes.Any(c => classIds.Contains(c.Id) || c.UserClasses.Any(uc => studentIds.Contains(uc.StudentId))));

    }
}
