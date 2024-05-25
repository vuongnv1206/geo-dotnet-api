using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions;
public class QuestionByIdSpec : Specification<Question>, ISingleResultSpecification
{
    public QuestionByIdRequestSpec(Guid id) =>
        Query
        .Where(x => x.Id == id)
        .Include(x => x.QuestionFolder)
        .Include(x => x.QuestionLable)
        .Include(x => x.QuestionPassages)
        .Include(x => x.Answers);
}
