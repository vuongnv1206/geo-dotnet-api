

namespace FSH.WebApi.Application.Questions.QuestionLabel;
public class QuestionLabelByNameSpec : Specification<Domain.Question.QuestionLable>, ISingleResultSpecification
{
    public QuestionLabelByNameSpec(string name)
    {
        Query.Where(x => x.Name == name);
    }
}
