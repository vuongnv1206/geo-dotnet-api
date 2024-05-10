using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperLabels;
public class SearchPaperLabelByNameSpec : Specification<PaperLabel>, ISingleResultSpecification
{
    public SearchPaperLabelByNameSpec(string name) =>
         Query.Where(p => p.Name == name);
}
