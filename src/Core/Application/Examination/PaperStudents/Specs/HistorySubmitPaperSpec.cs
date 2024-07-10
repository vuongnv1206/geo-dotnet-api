using Ardalis.Specification;
using FSH.WebApi.Application.Common.Specification;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperStudents;
public class HistorySubmitPaperSpec : EntitiesByPaginationFilterSpec<SubmitPaper>
{
    public HistorySubmitPaperSpec(GetHistoryTestOfStudentRequest request, Guid userId)
        : base(request)
    {
        Query
            .Where(sp => sp.CreatedBy == userId)
            .Include(sp => sp.Paper)
                .ThenInclude(p => p.PaperLabel)
            .Include(sp => sp.Paper)
                .ThenInclude(p => p.Subject);
    }
}
