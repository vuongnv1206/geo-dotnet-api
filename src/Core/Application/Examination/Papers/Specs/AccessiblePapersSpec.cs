

using FSH.WebApi.Domain.Examination;
using System.Linq;

namespace FSH.WebApi.Application.Examination.Papers.Specs;
internal class AccessiblePapersSpec : Specification<Paper>
{
    public AccessiblePapersSpec(IEnumerable<Guid> accessiblePaperIds, SearchSharedPaperRequest request)
    {
        Query
            .Include(x => x.PaperPermissions)
            .Include(x => x.PaperLabel)
            .Include(x => x.PaperFolder).ThenInclude(x => x.PaperFolderParent)
            .OrderBy(x => x.CreatedOn);

        if (accessiblePaperIds != null && accessiblePaperIds.Any())
        {
            Query.Where(x => accessiblePaperIds.Contains(x.Id));
        }
        else
        {
            Query.Where(x => false);
        }

        if (request.PaperFolderId.HasValue)
        {
            Query.Where(x => x.PaperFolderId == request.PaperFolderId);
        }
        else
        {
            Query.Where(x => x.PaperFolderId == null);
        }
    }
}
