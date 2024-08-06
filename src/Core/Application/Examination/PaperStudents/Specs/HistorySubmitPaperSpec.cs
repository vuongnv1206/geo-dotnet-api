using FSH.WebApi.Application.Examination.PaperStudents.Dtos;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperStudents.Specs;
public class HistorySubmitPaperSpec : EntitiesByPaginationFilterSpec<SubmitPaper, StudentTestHistoryDto>
{
    public HistorySubmitPaperSpec(GetHistoryTestOfStudentRequest request, DefaultIdType userId)
        : base(request)
    {
        _ = Query
            .Where(sp => sp.CreatedBy == userId)
            .Include(sp => sp.Paper)
                .ThenInclude(p => p.PaperLabel)
            .Include(sp => sp.Paper)
                .ThenInclude(p => p.Subject);
    }
}
