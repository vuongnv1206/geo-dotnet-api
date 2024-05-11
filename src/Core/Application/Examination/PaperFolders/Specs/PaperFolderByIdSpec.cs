using FSH.WebApi.Domain.Examination;


namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderByIdSpec : Specification<PaperFolder>, ISingleResultSpecification
{
    public PaperFolderByIdSpec(Guid id)
    {
        Query
        .Include(b => b.PaperFolderChildrens)
        .Where(b => b.Id == id);
    }
}
