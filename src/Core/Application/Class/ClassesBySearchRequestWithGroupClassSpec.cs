using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class;
public class ClassesBySearchRequestWithGroupClassSpec : EntitiesByPaginationFilterSpec<Classes, ClassDto>
{
    public ClassesBySearchRequestWithGroupClassSpec(SearchClassesRequest request, Guid userId)
        : base(request)
    {
        Query
            .Include(p => p.GroupClass)
            .OrderBy(c => c.Name, !request.HasOrderBy())
            .Where(p => p.GroupClassId.Equals(request.GroupClassId!.Value), request.GroupClassId.HasValue);

        Query.Where(p => p.OwnerId == userId);
    }
}
