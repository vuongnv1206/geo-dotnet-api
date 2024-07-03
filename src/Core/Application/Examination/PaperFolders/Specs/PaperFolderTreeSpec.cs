

using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderTreeSpec : Specification<PaperFolder>
{
    public PaperFolderTreeSpec()
    {
        Query
        .Include(x => x.PaperFolderParent)
        .Include(x => x.PaperFolderChildrens)
        .Include(x => x.PaperFolderPermissions)
        .ThenInclude(x => x.GroupTeacher)
        .Where(x => x.ParentId == request.ParentId)
        .Where(x => (x.CreatedBy == currentUserId || x.PaperFolderPermissions.Any(x => x.CanView)))
        .OrderBy(x => x.CreatedOn, !request.HasOrderBy());
    }
}
