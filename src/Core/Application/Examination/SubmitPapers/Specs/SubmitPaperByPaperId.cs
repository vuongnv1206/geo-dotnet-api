using FSH.WebApi.Application.Examination.SubmitPapers.Dtos;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class SubmitPaperByPaperId : Specification<SubmitPaper>
{
    public SubmitPaperByPaperId(Paper paper, Guid userId)
    {
        Query
          .Where(x => x.PaperId == paper.Id
          && (x.CreatedBy == userId || paper.CreatedBy == userId));
    }
}
