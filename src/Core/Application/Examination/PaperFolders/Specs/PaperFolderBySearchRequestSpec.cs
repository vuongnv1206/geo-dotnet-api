using FSH.WebApi.Application.Examination.PaperFolders.Dtos;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Examination.PaperFolders.Specs;
public class PaperFolderBySearchRequestSpec : Specification<PaperFolder>
{
    public PaperFolderBySearchRequestSpec(PaginationFilter request, Guid currentUserId) =>
        Query
            .Where(x => x.CreatedBy == currentUserId)
            .OrderBy(x => x.CreatedOn, !request.HasOrderBy())
            .Include(x => x.PaperFolderChildrens)
            .Include(x => x.PaperFolderParent);
}
