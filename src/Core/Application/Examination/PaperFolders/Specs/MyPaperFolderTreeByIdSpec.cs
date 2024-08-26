using FSH.WebApi.Domain.Examination;


namespace FSH.WebApi.Application.Examination.PaperFolders;
internal class MyPaperFolderTreeByIdSpec : Specification<PaperFolder>
{
    public MyPaperFolderTreeByIdSpec(DefaultIdType currentUserId)
    {
        Query
        .Include(x => x.PaperFolderParent)
        .Include(x => x.PaperFolderChildrens)
        .Include(x => x.PaperFolderPermissions)
        .Where(x => (x.CreatedBy == currentUserId || x.PaperFolderPermissions.Any(x => x.CanView)));
    }
}   

