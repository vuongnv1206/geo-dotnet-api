
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Papers;
public class PaperBySearchSpec : EntitiesByPaginationFilterSpec<Paper,PaperDto>
{
    public PaperBySearchSpec(SearchPaperRequest request) : base(request)
    {
        Query.OrderBy(c => c.ExamName, !request.HasOrderBy());
    }
}
