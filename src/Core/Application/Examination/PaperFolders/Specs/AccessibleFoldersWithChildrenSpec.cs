using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public sealed class AccessibleFoldersWithChildrenSpec : Specification<PaperFolder>
{
    public AccessibleFoldersWithChildrenSpec(IEnumerable<Guid> searchFolderIds, SearchSharedPaperFolderRequest request)
    {
        Query
            .Where(folder => searchFolderIds.Contains(folder.Id))
            .Include(x => x.PaperFolderParent)
            .Include(x => x.PaperFolderPermissions)
            .Include(folder => folder.PaperFolderChildrens);

        if (!string.IsNullOrEmpty(request.Name))
        {
            Query.Where(folder => folder.Name.ToLower().Contains(request.Name.ToLower()));
            //Query.Search(folder => folder.Name, "%" + request.Keyword + "%");
        }
        Query.OrderBy(x => x.CreatedOn);
    }
}
