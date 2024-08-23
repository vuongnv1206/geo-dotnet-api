
using FSH.WebApi.Application.Examination.SubmitPapers.Dtos;
using FSH.WebApi.Domain.Examination;
using MediatR;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class SubmitPaperByPaperIdPaging : EntitiesByPaginationFilterSpec<SubmitPaper, SubmitPaperDto>
{
    public SubmitPaperByPaperIdPaging(GetSubmittedPaperRequest request, Paper paper, Guid userId, List<Guid?> studentIds)
        : base(request)
    {
        Query
            .Include(sb => sb.Paper)
                .ThenInclude(p => p.PaperAccesses)
            .Where(x => x.PaperId == paper.Id
            && (x.CreatedBy == userId || paper.CreatedBy == userId))
            .Where(sb => !request.ClassId.HasValue || studentIds.Contains(sb.CreatedBy));
    }
}


