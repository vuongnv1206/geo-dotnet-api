

using FSH.WebApi.Domain.Examination;
using MediatR;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderBySearchSpec : Specification<PaperFolder>
{
    public PaperFolderBySearchSpec(IEnumerable<Guid> parentIds, SearchPaperFolderRequest request, Guid currentUserId)
    {
            Query
            .Include(x => x.PaperFolderParent)
            .Include(x => x.PaperFolderChildrens)
            .Include(x => x.PaperFolderPermissions)
            .ThenInclude(x => x.GroupTeacher)
            .Where(x => (x.CreatedBy == currentUserId || x.PaperFolderPermissions.Any(x => x.CanView))
                  && (string.IsNullOrEmpty(request.Name) || x.Name.ToLower().Contains(request.Name.ToLower())));

            if (parentIds.Any())
            {
                var nullableParentIds = parentIds.Select(id => (Guid?)id).ToList();
                Query.Where(x => nullableParentIds.Contains(x.ParentId) || nullableParentIds.Contains(x.Id));
            }

            Query.OrderBy(x => x.CreatedOn);
        
    }
}
