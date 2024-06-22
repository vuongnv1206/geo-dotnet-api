

using FSH.WebApi.Domain.Examination;
using System.Linq;

namespace FSH.WebApi.Application.Examination.Papers.Specs;
internal class AccessiblePapersSpec : EntitiesByPaginationFilterSpec<Paper>
{
    public AccessiblePapersSpec(IEnumerable<Guid> parentIds, IEnumerable<Guid> accessiblePaperIds, SearchSharedPaperRequest request)
        : base(request)
    {
        Query
             .Include(x => x.PaperPermissions)
            .Include(x => x.PaperLabel)
            .Include(x => x.PaperFolder).ThenInclude(x => x.PaperFolderParent)
            .OrderBy(x => x.CreatedOn, !request.HasOrderBy());

        if (parentIds != null && parentIds.Any())
        {
            var nullableIds = parentIds.Select(id => (Guid?)id).ToList();
            Query.Where(x => nullableIds.Contains(x.PaperFolderId) || nullableIds.Contains(x.Id));
        }
        if (accessiblePaperIds != null && accessiblePaperIds.Any())
        {
            Query.Where(x => accessiblePaperIds.Contains(x.Id));
        }

        if (!string.IsNullOrEmpty(request.Keyword))
        {
            Query.Where(x => x.ExamName.ToLower().Contains(request.Keyword.ToLower()));
        }
    }
}
