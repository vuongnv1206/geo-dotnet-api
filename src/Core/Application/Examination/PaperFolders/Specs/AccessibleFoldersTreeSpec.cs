using FSH.WebApi.Domain.Examination;
using MediatR;


namespace FSH.WebApi.Application.Examination.PaperFolders;
public sealed class AccessibleFoldersTreeSpec : EntitiesByPaginationFilterSpec<PaperFolder>
{
    public AccessibleFoldersTreeSpec(IEnumerable<Guid> accessibleFolderIds,SearchSharedPaperFolderRequest request)
         : base(request)
    {
        Query
            .Where(folder => accessibleFolderIds.Contains(folder.Id))
            .Include(x => x.PaperFolderParent)
            .Include(folder => folder.PaperFolderChildrens)
            .OrderBy(x => x.CreatedOn, !request.HasOrderBy());
        ;
    }
}