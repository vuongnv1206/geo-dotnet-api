using FSH.WebApi.Domain.Examination;
using MediatR;

namespace FSH.WebApi.Application.Examination.Papers;
public class PaperBySearchSpec : Specification<Paper>
{
    public PaperBySearchSpec(IEnumerable<Guid>? parentIds, SearchPaperRequest request, Guid currentUserId)
    {
        Query
            .Include(x => x.PaperPermissions)
            .Include(x => x.PaperLabel)
            .Include(x => x.PaperFolder).ThenInclude(x => x.PaperFolderParent)
            .Where(x => (x.CreatedBy == currentUserId))
            .OrderBy(x => x.CreatedOn);
           
        if (parentIds != null && parentIds.Any())
        {
            var nullableParentIds = parentIds.Select(id => (Guid?)id).ToList();
            Query.Where(x => nullableParentIds.Contains(x.PaperFolderId) || nullableParentIds.Contains(x.Id));
        }
        else if (!string.IsNullOrEmpty(request.Name))
        {
            Query.Where(x => x.ExamName.ToLower().Contains(request.Name.ToLower()));
        }
        else
        {
            Query.Where(x => x.PaperFolderId == null);
        }
    }

}
