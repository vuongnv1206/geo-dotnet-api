
using FSH.WebApi.Application.Examination.SubmitPapers.Dtos;
using FSH.WebApi.Domain.Examination;
using MediatR;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class SubmitPaperByPaperIdPaging : EntitiesByPaginationFilterSpec<SubmitPaper, SubmitPaperDto>
{
    public SubmitPaperByPaperIdPaging(GetSubmittedPaperRequest request, Paper paper, Guid userId, List<Guid?> studentIds, bool isTeacher)
        : base(request)
    {
        if (isTeacher)
        {
            Query.Include(sb => sb.Paper)
                .ThenInclude(p => p.PaperAccesses)
            .Where(x => x.PaperId == paper.Id && isTeacher).Where(sb => !request.ClassId.HasValue || studentIds.Contains(sb.CreatedBy))
            .Where(sb => !request.ClassId.HasValue || studentIds.Contains(sb.CreatedBy));
        }
        else
        {
            Query
                .Include(sb => sb.Paper)
                    .ThenInclude(p => p.PaperAccesses)
                .Where(x => x.PaperId == paper.Id
                && x.CreatedBy == userId)
                .Where(sb => !request.ClassId.HasValue || studentIds.Contains(sb.CreatedBy));
        }

    }
}


