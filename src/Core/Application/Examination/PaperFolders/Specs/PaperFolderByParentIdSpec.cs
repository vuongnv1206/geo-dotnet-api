using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderByParentIdSpec : Specification<PaperFolder>, ISingleResultSpecification
{
    public PaperFolderByParentIdSpec(Guid? id)
    {
        Query
       .Include(b => b.PaperFolderChildrens)
       .Where(b => b.ParentId == id);
    }
}
