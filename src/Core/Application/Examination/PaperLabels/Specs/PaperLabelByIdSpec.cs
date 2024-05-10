
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperLabels;
public class PaperLabelByIdSpec : Specification<PaperLable, PaperLabelDto>, ISingleResultSpecification
{
    public PaperLabelByIdSpec(Guid id)
    {
        Query.Where(p => p.Id == id);
    }
}
