using FSH.WebApi.Domain.Examination;
using MediatR;


namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderBySearchRequestWithParentIdSpec : Specification<PaperFolder>
{
    public PaperFolderBySearchRequestWithParentIdSpec(DefaultIdType currentUserId)
    {
        Query
        .Include(x => x.PaperFolderParent)
        .Include(x => x.PaperFolderChildrens)
        .Where(x => (x.CreatedBy == currentUserId || x.PaperFolderPermissions.Any(x => x.CanView)))
        .OrderBy(x => x.CreatedOn);
    }



}
