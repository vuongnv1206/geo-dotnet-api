using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions.Specs;
public class QuestionFolderByIdSpec : Specification<QuestionFolder>, ISingleResultSpecification
{
    public QuestionFolderByIdSpec(Guid? id) =>
        Query
        .Include(b => b.Permissions)
        .Include(b => b.Children)
        .Include(x => x.Questions).ThenInclude(x => x.QuestionLable)
        .Where(b => b.Id == id);
}