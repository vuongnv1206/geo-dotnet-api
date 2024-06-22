using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class SubmitPaperByIdSpec : Specification<SubmitPaper>, ISingleResultSpecification
{
    public SubmitPaperByIdSpec(Guid id)
    {
        Query
            .Include(x => x.Paper)
            .ThenInclude(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.Answers)
            .Include(x => x.Paper)
            .ThenInclude(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.QuestionPassages).ThenInclude(qp => qp.Answers)
            .Include(x => x.SubmitPaperDetails)
                .ThenInclude(x => x.Question)
            .Where(x => x.Id == id);
    }
}
