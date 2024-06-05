using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions.Specs;
public class QuestionFolderPermissionByFolderIdAndTeacherGroupIdSpec : Specification<QuestionFolderPermission>, ISingleResultSpecification
{
    public QuestionFolderPermissionByFolderIdAndTeacherGroupIdSpec(Guid folderId, Guid teacherGroupId)
    {
        Query.Where(x => x.QuestionFolderId == folderId && x.GroupTeacherId == teacherGroupId);
    }
}