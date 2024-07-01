

using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderTreeSpec : Specification<PaperFolder>
{
    public PaperFolderTreeSpec()
    {
        Query
       .Include(b => b.PaperFolderChildrens)
       .Include(x => x.PaperFolderPermissions)
       .Include(x => x.Papers).ThenInclude(x => x.PaperPermissions);
    }
}
