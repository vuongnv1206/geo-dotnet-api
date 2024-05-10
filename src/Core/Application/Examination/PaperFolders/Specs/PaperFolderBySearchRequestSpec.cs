using FSH.WebApi.Domain.Examination;


namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderBySearchRequestSpec : Specification<PaperFolder>
{
    public PaperFolderBySearchRequestSpec(SearchPaperFolderRequest request, DefaultIdType currentUserId) =>
        Query
            .Where(x => (x.CreatedBy == currentUserId || x.PaperFolderPermissions.Any(x => x.CanView))
                     && x.ParentId == request.ParentId
                     && (string.IsNullOrEmpty(request.Name) || x.Name.Contains(request.Name)))
            .OrderBy(x => x.CreatedOn)
            .Include(x => x.PaperFolderChildrens)
            .Include(x => x.PaperFolderParent);
}
