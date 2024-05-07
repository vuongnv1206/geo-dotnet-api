using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions;
public class QuestionFolderByIdSpec : Specification<QuestionFolder>, ISingleResultSpecification
{
    public QuestionFolderByIdSpec(Guid id) =>
        Query.Where(b => b.Id == id);
}