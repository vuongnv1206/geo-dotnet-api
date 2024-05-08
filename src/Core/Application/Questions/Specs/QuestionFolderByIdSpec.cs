using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions.Specs;
public class QuestionFolderByIdSpec : Specification<QuestionFolder>, ISingleResultSpecification
{
    public QuestionFolderByIdSpec(Guid? id) =>
        Query
        .Include(b => b.Permissions)
        .Include(b => b.Children)
        .Where(b => b.Id == id);
}