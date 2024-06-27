using FSH.WebApi.Domain.Examination;


namespace FSH.WebApi.Application.Examination.PaperFolders;
public sealed class AccessibleFoldersTreeSpec : Specification<PaperFolder>
{
    public AccessibleFoldersTreeSpec(IEnumerable<Guid> accessibleFolderIds,SearchSharedPaperFolderRequest request)
    {
        Query
            
            .Include(x => x.PaperFolderParent)
            .Include(folder => folder.PaperFolderChildrens)
            .Where(folder => accessibleFolderIds.Contains(folder.Id))
            .Where(x => x.ParentId == request.ParentId)
            .OrderBy(x => x.CreatedOn);
        
    }
}