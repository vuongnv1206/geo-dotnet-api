using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Papers;
public class PaperByIdSpec : Specification<Paper>, ISingleResultSpecification
{
    public PaperByIdSpec(Guid id)
    {
        Query.Where(p => p.Id == id)
            .Include(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.AnswerClones)
            .Include(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.QuestionPassages).ThenInclude(qp => qp.AnswerClones)
            .Include(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.QuestionLabel)
            .Include(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.QuestionFolder)
            .Include(x => x.Subject)
            .Include(x => x.SubmitPapers)
            .Include(x => x.PaperAccesses).ThenInclude(pa => pa.Class).ThenInclude(c => c.UserClasses).ThenInclude(uc => uc.Student)
            .Include(x => x.PaperPermissions);
    }
}
