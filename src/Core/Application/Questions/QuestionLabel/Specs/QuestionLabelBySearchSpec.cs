

using FSH.WebApi.Application.Questions.QuestionLabel.Dtos;
using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions.QuestionLabel;
public class QuestionLabelBySearchSpec : EntitiesByPaginationFilterSpec<QuestionLable, QuestionLabelDto>
{
    public QuestionLabelBySearchSpec(SearchQuestionLabelRequest request)
         : base(request)
    {
        Query.OrderBy(c => c.Name, !request.HasOrderBy());
    }
}
