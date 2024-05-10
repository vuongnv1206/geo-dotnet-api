using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions.Specs;
public class QuestionFolderPermissionByFolderIdAndUserIdSpec : Specification<QuestionFolderPermission>, ISingleResultSpecification
{
    public QuestionFolderPermissionByFolderIdAndUserIdSpec(Guid folderId, Guid userId)
    {
        Query.Where(x => x.QuestionFolderId == folderId && x.UserId == userId);
    }
}