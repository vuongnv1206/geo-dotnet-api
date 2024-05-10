using FSH.WebApi.Domain.Examination;


namespace FSH.WebApi.Application.Examination.PaperLabels;
public class PaperLabelByNameSpec : Specification<PaperLabel>, ISingleResultSpecification
{
    public PaperLabelByNameSpec(string name)
    {
        Query.Where(x => x.Name == name);
    }
}
