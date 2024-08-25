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
       .Include(x => x.Papers).ThenInclude(x => x.SubmitPapers)
       .Include(x => x.Papers).ThenInclude(x => x.PaperPermissions)
       .ThenInclude(x => x.GroupTeacher)
       .OrderBy(x => x.CreatedOn);
    }
}
