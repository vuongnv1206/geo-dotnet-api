using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderByNameSpec : Specification<PaperFolder>, ISingleResultSpecification
{
    public PaperFolderByNameSpec(string name,Guid? parentId)
    {
        if (parentId.HasValue)
        {
            Query.Where(x => x.ParentId == parentId);
        }
        Query.Where(b => b.Name == name);
       
    }
}
