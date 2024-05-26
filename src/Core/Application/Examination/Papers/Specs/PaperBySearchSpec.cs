using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Papers;
public class PaperBySearchSpec : Specification<Paper>
{
    public PaperBySearchSpec(SearchPaperRequest request)
    {
        if (!string.IsNullOrEmpty(request.Name))
        {
            Query
                .Where(x => x.ExamName.Contains(request.Name))
                .Include(x => x.PaperLable)
                .Include(x => x.PaperFolder)
                .OrderBy(c => c.ExamName);
        }
        else
        {
            Query
             .Where(x => request.PaperFolderId == null || x.PaperFolderId == request.PaperFolderId)
             .Include(x => x.PaperLable)
             .Include(x => x.PaperFolder)
             .OrderBy(c => c.ExamName);
        }
    }
}
