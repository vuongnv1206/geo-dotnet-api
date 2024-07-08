

using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class MyPaperFolderTreeSpec : Specification<PaperFolder>
{
    public MyPaperFolderTreeSpec(Guid currentUserId)
    {
        Query
        .Include(x => x.PaperFolderParent)
        .Include(x => x.PaperFolderChildrens)
        .Include(x => x.PaperFolderPermissions)
        .ThenInclude(x => x.GroupTeacher)
        .Where(x => x.CreatedBy == currentUserId)
        .OrderBy(x => x.CreatedOn);
    }
}
