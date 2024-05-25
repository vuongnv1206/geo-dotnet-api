using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions;
public class QuestionLabelByIdSpec : Specification<QuestionLable>, ISingleResultSpecification
{
    public QuestionLabelByIdSpec(Guid? id) =>
        Query
        .Where(x => x.Id == id);
}
