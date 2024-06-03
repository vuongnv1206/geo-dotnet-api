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
            .OrderBy(c => c.Name, !request.HasOrderBy())
            .Where(p => p.GroupClassId.Equals(request.GroupClassId!.Value), request.GroupClassId.HasValue)
            .Where(x => string.IsNullOrEmpty(request.Keyword)
                        || x.Name.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase)
                        || x.SchoolYear.Equals(request.Keyword, StringComparison.OrdinalIgnoreCase))
            .Where(p => p.CreatedBy == userId);
    }
}
