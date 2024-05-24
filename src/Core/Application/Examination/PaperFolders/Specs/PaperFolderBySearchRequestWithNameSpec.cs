using FSH.WebApi.Domain.Examination;


namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderBySearchRequestWithNameSpec : Specification<PaperFolder>
{
    public PaperFolderBySearchRequestWithNameSpec(string name, DefaultIdType currentUserId)
    {
        Query
            .Where(x => (x.CreatedBy == currentUserId || x.PaperFolderPermissions.Any(x => x.CanView))
                  && !string.IsNullOrEmpty(name) && x.Name.Contains(name))
            .OrderBy(x => x.CreatedOn);
    }
}
