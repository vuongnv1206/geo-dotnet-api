using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Papers;
public class PaperByIdWithAccessesSpec : Specification<Paper>, ISingleResultSpecification
{
    public PaperByIdWithAccessesSpec(Guid id)
    {
        Query.Where(p => p.Id == id)
            .Include(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.AnswerClones)
            .Include(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.QuestionPassages).ThenInclude(qp => qp.AnswerClones)
            .Include(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.QuestionLabel)
            .Include(p => p.PaperAccesses).ThenInclude(pa => pa.Class).ThenInclude(c => c.UserClasses)
            .Include(x => x.SubmitPapers)
            .Include(x => x.PaperAccesses)
            .Include(x => x.PaperPermissions);
    }
}
