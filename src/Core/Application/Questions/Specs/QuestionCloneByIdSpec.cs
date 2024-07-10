using FSH.WebApi.Domain.Question;


namespace FSH.WebApi.Application.Questions;
public class QuestionCloneByIdSpec : Specification<QuestionClone>, ISingleResultSpecification
{
    public QuestionCloneByIdSpec(Guid id)
    {
        Query
            .Where(x => x.Id == id)
            .Include(x => x.QuestionFolder)
            .Include(x => x.QuestionLabel)
            .Include(x => x.QuestionPassages)
            .Include(x => x.AnswerClones);
    }
}
