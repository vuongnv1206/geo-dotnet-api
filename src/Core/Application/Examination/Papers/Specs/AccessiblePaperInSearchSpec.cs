using FSH.WebApi.Domain.Examination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.Papers.Specs;
public class AccessiblePaperInSearchSpec : Specification<Paper>
{
    public AccessiblePaperInSearchSpec(IEnumerable<Guid> accessiblePaperIds, IEnumerable<Guid> searchableFolderIds, SearchSharedPaperRequest request)
    {
        Query
              .Include(x => x.PaperPermissions)
              .Include(x => x.PaperLabel)
              .Include(x => x.PaperFolder).ThenInclude(x => x.PaperFolderParent)
              .Include(x => x.PaperFolder).ThenInclude(x => x.PaperFolderPermissions)
              .Where(x => accessiblePaperIds.Contains(x.Id)); // Filter by accessible paper IDs

        if (searchableFolderIds != null && searchableFolderIds.Any())
        {
            Query.Where(x => searchableFolderIds.Contains(x.PaperFolderId.Value)); // Filter by searchable folder IDs
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            Query.Where(x => x.ExamName.ToLower().Contains(request.Name.ToLower())); // Filter by name if provided
        }

        Query.OrderBy(x => x.CreatedOn);
    }
}
