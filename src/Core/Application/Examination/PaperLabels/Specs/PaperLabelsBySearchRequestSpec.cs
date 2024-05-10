
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperLabels;
public class PaperLabelsBySearchRequestSpec : EntitiesByPaginationFilterSpec<PaperLabel, PaperLabelDto>
{
    public PaperLabelsBySearchRequestSpec(SearchPaperLabelRequest request) : base(request)
    {
        Query.OrderBy(c => c.Name, !request.HasOrderBy());
    }
}
