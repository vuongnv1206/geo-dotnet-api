using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class QuestionCloneInPaperSpec : EntitiesByPaginationFilterSpec<QuestionClone>
{
    public QuestionCloneInPaperSpec(GetQuestionStatisticRequest request, List<Guid> questionIds)
        : base(request)
    {
        Query.Where(q => questionIds.Contains(q.Id));
    }
}
