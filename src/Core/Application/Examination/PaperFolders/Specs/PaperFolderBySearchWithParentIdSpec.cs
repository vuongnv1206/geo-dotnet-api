using FSH.WebApi.Domain.Examination;


namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderBySearchWithParentIdSpec : Specification<PaperFolder>
{
    public PaperFolderBySearchWithParentIdSpec(DefaultIdType currentUserId)
    {
        Query
        .Include(x => x.PaperFolderParent)
        .Include(x => x.PaperFolderChildrens)
        .Where(x => (x.CreatedBy == currentUserId || x.PaperFolderPermissions.Any(x => x.CanView)))
        .OrderBy(x => x.CreatedOn);
    }
}
