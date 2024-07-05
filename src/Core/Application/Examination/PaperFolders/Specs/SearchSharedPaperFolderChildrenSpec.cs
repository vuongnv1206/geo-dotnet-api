using FSH.WebApi.Domain.Examination;


namespace FSH.WebApi.Application.Examination.PaperFolders;
public class SearchSharedPaperFolderChildrenSpec : Specification<PaperFolder>
{
    public SearchSharedPaperFolderChildrenSpec(IEnumerable<Guid> accessibleFolderIds)
    {
        Query
            .Include(x => x.PaperFolderParent)
            .Include(folder => folder.PaperFolderChildrens)
            .Where(folder => accessibleFolderIds.Contains(folder.Id) && folder.ParentId.HasValue && !accessibleFolderIds.Contains(folder.ParentId.Value))
            .OrderBy(x => x.CreatedOn);
    }
}
