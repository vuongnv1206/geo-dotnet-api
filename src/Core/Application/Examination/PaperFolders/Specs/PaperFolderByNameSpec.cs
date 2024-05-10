using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderByNameSpec : Specification<PaperFolder>, ISingleResultSpecification
{
    public PaperFolderByNameSpec(string name)
    {
        Query.Where(b => b.Name == name);
    }
}
