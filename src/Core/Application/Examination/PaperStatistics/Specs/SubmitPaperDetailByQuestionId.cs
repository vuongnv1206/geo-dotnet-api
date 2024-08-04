using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class SubmitPaperDetailByQuestionId : Specification<SubmitPaperDetail>
{
    public SubmitPaperDetailByQuestionId(Guid questionId, Guid paperId)
    {
        Query.Where(x => x.QuestionId == questionId)
            .Include(x => x.SubmitPaper)
            .Where(x => x.SubmitPaper.PaperId == paperId);
    }
}
