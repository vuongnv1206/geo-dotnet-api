using FSH.WebApi.Domain.Question;


namespace FSH.WebApi.Application.Questions.QuestionLabel;
public class QuestionLabelByIdSpec : Specification<QuestionLable>
{
    public QuestionLabelByIdSpec(Guid id)
    {
        Query.Where(x => x.Id == id);
    }
}
