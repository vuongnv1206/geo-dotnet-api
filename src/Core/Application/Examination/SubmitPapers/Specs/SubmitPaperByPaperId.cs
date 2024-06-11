using FSH.WebApi.Domain.Examination;
using MediatR;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class SubmitPaperByPaperId : EntitiesByPaginationFilterSpec<SubmitPaper, SubmitPaperDto>
{
    public SubmitPaperByPaperId(GetSubmittedPaperRequest request, Paper paper, Guid userId)
        : base(request)
    {
        Query
            .Where(x => x.PaperId == paper.Id
            && ( x.CreatedBy == userId || paper.CreatedBy == userId));
    }
}
