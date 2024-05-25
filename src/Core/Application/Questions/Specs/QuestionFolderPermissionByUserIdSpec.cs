using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions;
public class QuestionFolderPermissionByUserIdSpec : Specification<QuestionFolderPermission>, ISingleResultSpecification
{
    public QuestionFolderPermissionByUserIdSpec(DefaultIdType userId) =>
        Query.Where(q => q.UserId == userId);
}