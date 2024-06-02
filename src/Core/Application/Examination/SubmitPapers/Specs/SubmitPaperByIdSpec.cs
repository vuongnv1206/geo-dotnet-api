using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class SubmitPaperByIdSpec : Specification<SubmitPaper>, ISingleResultSpecification
{
    public SubmitPaperByIdSpec(Guid id)
    {
        Query
            .Include(x => x.Paper)
            .Include(x => x.SubmitPaperDetails)
            .Where(x => x.Id == id);
    }
}
