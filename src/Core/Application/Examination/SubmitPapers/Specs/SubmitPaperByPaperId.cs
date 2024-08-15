using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.SubmitPapers.Specs;
public class SubmitPaperByPaperId : Specification<SubmitPaper>
{
    public SubmitPaperByPaperId(Paper paper, DefaultIdType userId)
    {
        _ = Query
          .Where(x => x.PaperId == paper.Id
          && (x.CreatedBy == userId || paper.CreatedBy == userId))
          .Include(sb => sb.SubmitPaperDetails)
          .OrderBy(x => x.CreatedOn);
    }
}
