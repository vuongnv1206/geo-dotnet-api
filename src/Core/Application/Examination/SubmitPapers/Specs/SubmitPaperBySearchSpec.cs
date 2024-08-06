

using FSH.WebApi.Application.Examination.PaperStatistics;
using FSH.WebApi.Domain.Examination;
using MediatR;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class SubmitPaperBySearchSpec : EntitiesByPaginationFilterSpec<SubmitPaper>
{
    public SubmitPaperBySearchSpec(GetListTranscriptRequest request, IEnumerable<Guid> accessibleIds)
               : base(request)
    {
        Query
            .Include(x => x.Paper)
            .Include(x => x.SubmitPaperDetails)
            .Where(x => x.PaperId == request.PaperId);

        if (request.ClassId.HasValue)
        {
            Query
            .Include(x => x.Paper)
            .ThenInclude(p => p.PaperAccesses)
            .ThenInclude(pa => pa.Class)
            .ThenInclude(c => c.UserClasses)
            .ThenInclude(uc => uc.Student)
            .Where(x => x.Paper.PaperAccesses.FirstOrDefault(pa => pa.ClassId == request.ClassId.Value).Class.UserClasses.Any(uc => uc.Student.StId == x.CreatedBy));
        }

        if (accessibleIds.Any())
        {
            Query.Where(x => accessibleIds.Contains(x.CreatedBy));
        }
        Query.OrderBy(x => x.CreatedOn, !request.HasOrderBy());
    }
}
