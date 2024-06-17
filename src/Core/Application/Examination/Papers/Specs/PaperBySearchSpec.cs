using FSH.WebApi.Domain.Examination;
using MediatR;

namespace FSH.WebApi.Application.Examination.Papers;
public class PaperBySearchSpec : EntitiesByPaginationFilterSpec<Paper>
{
    public PaperBySearchSpec(IEnumerable<Guid>? parentIds, SearchPaperRequest request)
        : base(request)
    {
        Query
            .Include(x => x.PaperLabel)
            .Include(x => x.PaperFolder).ThenInclude(x => x.PaperFolderParent)
            .OrderBy(x => x.CreatedOn, !request.HasOrderBy());

        if (parentIds != null && parentIds.Any())
        {
            var nullableParentIds = parentIds.Select(id => (Guid?)id).ToList();
            Query.Where(x => nullableParentIds.Contains(x.PaperFolderId) || nullableParentIds.Contains(x.Id));
        }
        else if (!string.IsNullOrEmpty(request.Keyword))
        {
            Query.Where(x => x.ExamName.ToLower().Contains(request.Keyword.ToLower()));
        }
        else
        {
            Query.Where(x => x.PaperFolderId == null);
        }
    }

}
