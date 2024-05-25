using FSH.WebApi.Domain.Examination;


namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderBySearchRequestSpec : Specification<PaperFolder>
{
    public PaperFolderBySearchRequestSpec(SearchPaperFolderRequest request, DefaultIdType currentUserId) =>
        Query
        .Include(x => x.PaperFolderParent)
        .Include(x => x.PaperFolderChildrens)
            .Where(x => (x.CreatedBy == currentUserId || x.PaperFolderPermissions.Any(x => x.CanView))
                     && (string.IsNullOrEmpty(request.Name) || x.Name.Contains(request.Name)))
            .OrderBy(x => x.CreatedOn);
}
