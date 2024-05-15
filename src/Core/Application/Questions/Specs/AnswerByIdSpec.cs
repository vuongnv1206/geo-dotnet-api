using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions.Specs;
public class AnswerByIdSpec : Specification<Answer>, ISingleResultSpecification
{
    public AnswerByIdSpec(Guid? id) =>
        Query
        .Where(x => x.Id == id)
        .Include(x => x.Question);
}
