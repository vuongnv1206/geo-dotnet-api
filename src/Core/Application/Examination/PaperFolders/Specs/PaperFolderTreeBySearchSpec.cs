using FSH.WebApi.Domain.Examination;
using MediatR;


namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderTreeBySearchSpec : EntitiesByPaginationFilterSpec<PaperFolder>
{
    public PaperFolderTreeBySearchSpec(DefaultIdType currentUserId, SearchPaperFolderRequest request)
        : base(request)
    {
        Query
        .Include(x => x.PaperFolderParent)
        .Include(x => x.PaperFolderChildrens)
        .Where(x => x.ParentId == request.ParentId)
        .Where(x => (x.CreatedBy == currentUserId))
        .OrderBy(x => x.CreatedOn, !request.HasOrderBy());
    }
}
