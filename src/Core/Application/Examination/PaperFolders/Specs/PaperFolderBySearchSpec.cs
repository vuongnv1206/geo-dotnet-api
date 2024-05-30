

using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderBySearchSpec : Specification<PaperFolder>
{
    public PaperFolderBySearchSpec(IEnumerable<Guid> parentIds, string name, DefaultIdType currentUserId)
    {
            Query
            .Include(x => x.PaperFolderParent)
            .Include(x => x.PaperFolderChildrens)
            .Where(x => (x.CreatedBy == currentUserId || x.PaperFolderPermissions.Any(x => x.CanView))
                  && (string.IsNullOrEmpty(name) || x.Name.ToLower().Contains(name.ToLower())));

            if (parentIds.Any())
            {
                var nullableParentIds = parentIds.Select(id => (Guid?)id).ToList();
                Query.Where(x => nullableParentIds.Contains(x.ParentId) || nullableParentIds.Contains(x.Id));
            }

            Query.OrderBy(x => x.CreatedOn);
        
    }
}
