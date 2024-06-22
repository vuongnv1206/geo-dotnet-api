using FSH.WebApi.Domain.Examination;


namespace FSH.WebApi.Application.Examination.PaperFolders;
internal class PaperFolderTreeByIdSpec : Specification<PaperFolder>
{
    public PaperFolderTreeByIdSpec(DefaultIdType currentUserId)
    {
        Query
        .Include(x => x.PaperFolderParent)
        .Include(x => x.PaperFolderChildrens)
        .Where(x => (x.CreatedBy == currentUserId || x.PaperFolderPermissions.Any(x => x.CanView)));
    }
}   

