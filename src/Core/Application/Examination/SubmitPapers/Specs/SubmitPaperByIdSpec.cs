﻿using FSH.WebApi.Application.Examination.SubmitPapers.Dtos;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class SubmitPaperByIdSpec : Specification<SubmitPaper>, ISingleResultSpecification
{
    public SubmitPaperByIdSpec(Guid id)
    {
        Query
            .Include(x => x.Paper)
            .ThenInclude(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.AnswerClones)
            .Include(x => x.Paper)
            .ThenInclude(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.QuestionPassages).ThenInclude(qp => qp.AnswerClones)
            .Include(x => x.SubmitPaperDetails)
                .ThenInclude(x => x.Question)
            .Where(x => x.Id == id);
    }
}
