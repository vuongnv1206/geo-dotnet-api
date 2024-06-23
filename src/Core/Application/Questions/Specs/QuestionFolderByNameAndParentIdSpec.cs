using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions.Specs;
public class QuestionFolderByNameAndParentIdSpec : Specification<QuestionFolder>, ISingleResultSpecification
{
    public QuestionFolderByNameAndParentIdSpec(string name, Guid? parentId) =>
        Query
        .Where(b => b.Name == name && b.ParentId == parentId);
}