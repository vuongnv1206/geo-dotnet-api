

using FSH.WebApi.Domain.Examination;
using MediatR;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderBySearchSpec : EntitiesByPaginationFilterSpec<PaperFolder>
{
    public PaperFolderBySearchSpec(IEnumerable<Guid> parentIds, SearchPaperFolderRequest request, DefaultIdType currentUserId)
        : base(request)
    {
            Query
            .Include(x => x.PaperFolderParent)
            .Include(x => x.PaperFolderChildrens)
            .Where(x => (x.CreatedBy == currentUserId || x.PaperFolderPermissions.Any(x => x.CanView))
                  && (string.IsNullOrEmpty(request.Keyword) || x.Name.ToLower().Contains(request.Keyword.ToLower())));

            if (parentIds.Any())
            {
                var nullableParentIds = parentIds.Select(id => (Guid?)id).ToList();
                Query.Where(x => nullableParentIds.Contains(x.ParentId) || nullableParentIds.Contains(x.Id));
            }

            Query.OrderBy(x => x.CreatedOn, !request.HasOrderBy());
        
    }
}
