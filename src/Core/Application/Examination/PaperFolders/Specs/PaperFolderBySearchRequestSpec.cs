using FSH.WebApi.Application.Examination.PaperFolders.Dtos;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Examination.PaperFolders.Specs;
public class PaperFolderBySearchRequestSpec : Specification<PaperFolder>
{
    public PaperFolderBySearchRequestSpec(SearchPaperFolderRequest request, DefaultIdType currentUserId) =>
        Query
            .Where(x => x.CreatedBy == currentUserId
                     && x.ParentId == request.ParentId
                     && (string.IsNullOrEmpty(request.Name) || x.Name.Contains(request.Name)))
            .OrderBy(x => x.CreatedOn)
            .Include(x => x.PaperFolderChildrens)
            .Include(x => x.PaperFolderParent);
}
