using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class;
public class ClassesBySearchRequestWithGroupClassSpec : EntitiesByPaginationFilterSpec<Classes, ClassDto>
{
    public ClassesBySearchRequestWithGroupClassSpec(SearchClassesRequest request, DefaultIdType userId)
        : base(request)
    {
        Query
            .Include(p => p.GroupClass)
            .Include(a => a.AssignmentClasses)
                .ThenInclude(a => a.Assignment)
            .Include(u => u.UserClasses)
                .ThenInclude(x => x.Student)
            .Include(x => x.PaperAccesses)
                .ThenInclude(x => x.Paper)
            .Where(p => p.CreatedBy == userId &&
            (!request.GroupClassId.HasValue || p.GroupClassId == request.GroupClassId));
    }
}
