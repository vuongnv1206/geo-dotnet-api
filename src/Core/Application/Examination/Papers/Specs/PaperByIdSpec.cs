using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Papers;
public class PaperByIdSpec : Specification<Paper>, ISingleResultSpecification
{
    public PaperByIdSpec(Guid id)
    {
        Query.Where(p => p.Id == id)
            .Include(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.Answers)
            .Include(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.QuestionPassages).ThenInclude(qp => qp.Answers)
            .Include(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.QuestionLable);
    }
}
