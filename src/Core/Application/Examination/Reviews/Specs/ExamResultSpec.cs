

using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Reviews;
public class ExamResultSpec : Specification<SubmitPaper>, ISingleResultSpecification
{
    public ExamResultSpec(Guid id, Paper paper, Guid studentId, Guid currentId)
    {
        // studentId là bài của học sinh nào, currentId là nguwoif đang truy cập
        // nếu à học sinh thì phải student thì cần phải check xem đã có quyền xem detail k
        Query.Where(x => x.Id == id && x.PaperId == paper.Id && x.CreatedBy == studentId)
            .Include(x => x.Paper).ThenInclude(x => x.PaperQuestions).ThenInclude(x => x.Question).ThenInclude(x => x.AnswerClones)
            .Include(x => x.Paper).ThenInclude(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.QuestionPassages).ThenInclude(qp => qp.AnswerClones)
            .Include(x => x.Paper).ThenInclude(p => p.PaperQuestions).ThenInclude(pq => pq.Question).ThenInclude(q => q.QuestionLabel)
            .Include(x => x.Paper).ThenInclude(p => p.SubmitPapers)
            .Include(x => x.Paper).ThenInclude(p => p.PaperAccesses).ThenInclude(pa => pa.Class).ThenInclude(c => c.UserClasses)
            .Include(x => x.SubmitPaperDetails).ThenInclude(x => x.Question).ThenInclude(x => x.AnswerClones)
            .Include(x => x.SubmitPaperDetails).ThenInclude(x => x.Question).ThenInclude(q => q.QuestionPassages).ThenInclude(x => x.AnswerClones).AsSplitQuery();
    }
}
