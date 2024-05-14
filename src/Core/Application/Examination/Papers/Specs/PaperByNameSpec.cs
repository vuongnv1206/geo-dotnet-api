

using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Papers;
public class PaperByNameSpec : Specification<Paper> ,ISingleResultSpecification
{
    public PaperByNameSpec(string name) =>
        Query.Where(b => b.ExamName == name);
}
