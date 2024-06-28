using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderByIdSpec : Specification<PaperFolder>, ISingleResultSpecification
{
    public PaperFolderByIdSpec(Guid id)
    {
        Query
        .Include(b => b.PaperFolderChildrens)
        .Include(p => p.PaperFolderPermissions)
        .Include(x => x.Papers)
        .Where(b => b.Id == id);
    }
}
