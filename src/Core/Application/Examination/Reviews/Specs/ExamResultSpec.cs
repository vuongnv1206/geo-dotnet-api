﻿

using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Reviews;
public class ExamResultSpec : Specification<SubmitPaper>, ISingleResultSpecification
{
    public ExamResultSpec(Guid id, Guid paperId, Guid userId)
    {
        Query.Where(x => x.Id == id && x.PaperId == paperId && x.CreatedBy == userId)
            .Include(x => x.Paper).ThenInclude(x => x.PaperQuestions).ThenInclude(x => x.Question).ThenInclude(x => x.Answers)
            .Include(x => x.Paper).ThenInclude(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.QuestionPassages).ThenInclude(qp => qp.Answers)
            .Include(x => x.Paper).ThenInclude(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.QuestionLable)
            .Include(x => x.SubmitPaperDetails).ThenInclude(x => x.Question).ThenInclude(x => x.Answers);
    }
}
