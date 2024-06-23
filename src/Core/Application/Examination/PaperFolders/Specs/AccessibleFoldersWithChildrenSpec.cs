using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public sealed class AccessibleFoldersWithChildrenSpec : EntitiesByPaginationFilterSpec<PaperFolder>
{
    public AccessibleFoldersWithChildrenSpec(IEnumerable<Guid> searchFolderIds, SearchSharedPaperFolderRequest request)
          : base(request)
    {
        Query
            .Where(folder => searchFolderIds.Contains(folder.Id))
            .Include(x => x.PaperFolderParent)
            .Include(folder => folder.PaperFolderChildrens);

        if (!string.IsNullOrEmpty(request.Keyword))
        {
            Query.Where(folder => folder.Name.ToLower().Contains(request.Keyword.ToLower()));
            //Query.Search(folder => folder.Name, "%" + request.Keyword + "%");
        }
        Query.OrderBy(x => x.CreatedOn, !request.HasOrderBy());
    }
}
