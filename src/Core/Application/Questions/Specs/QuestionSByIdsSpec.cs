using FSH.WebApi.Domain.Question;


namespace FSH.WebApi.Application.Questions;
public class QuestionsByIdsSpec : Specification<Question>, ISingleResultSpecification
{
    public QuestionsByIdsSpec(IEnumerable<Guid> ids)
    {
        Query.Where(q => ids.Contains(q.Id));
    }
}
