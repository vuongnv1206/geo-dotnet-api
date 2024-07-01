using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderByIdSpec : Specification<PaperFolder>, ISingleResultSpecification
{
    public PaperFolderByIdSpec(Guid id)
    {
        Query
        .Include(x => x.PaperFolderPermissions)
        .Include(b => b.PaperFolderChildrens)
        .Include(x => x.Papers).ThenInclude(x => x.PaperPermissions)
        .Where(b => b.Id == id);
    }
}
