

using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class SubmitPaperByUserIdsSpec : Specification<SubmitPaper>
{
    public SubmitPaperByUserIdsSpec(Guid paperId,List<Guid> accessibleIds)
    {
        Query
            .Where(x => x.PaperId == paperId)
            .OrderByDescending(x => x.CreatedOn);

        if (accessibleIds.Any())
        {
            Query.Where(x => accessibleIds.Contains(x.CreatedBy));
        }
    }
}
