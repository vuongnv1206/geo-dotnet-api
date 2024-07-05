using FSH.WebApi.Domain.Examination;


namespace FSH.WebApi.Application.Examination.Papers;
public class SharedPapersInChildFolderSpec : Specification<Paper>
{
    public SharedPapersInChildFolderSpec(IEnumerable<Guid> accessiblePaperIds,IEnumerable<Guid> accessibleFolderIds, SearchSharedPaperRequest request)
    {
        Query.Include(x => x.PaperPermissions)
             .Include(x => x.PaperLabel)
             .Include(x => x.PaperFolder).ThenInclude(x => x.PaperFolderParent)
             .Include(x => x.PaperFolder).ThenInclude(x => x.PaperFolderChildrens)
             .Where(paper => paper.PaperFolderId.HasValue &&
                             !accessibleFolderIds.Contains(paper.PaperFolderId.Value) && 
                             accessiblePaperIds.Contains(paper.Id)); 

        if (!string.IsNullOrEmpty(request.Name))
        {
            Query.Where(x => x.ExamName.ToLower().Contains(request.Name.ToLower()));
        }

        Query.OrderBy(x => x.CreatedOn);

    }
}
