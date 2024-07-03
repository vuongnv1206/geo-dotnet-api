using FSH.WebApi.Domain.Question;
using System.Linq;

namespace FSH.WebApi.Application.Questions.Specs;
public class QuestionFoldersWithPermissionsSpecByUserId : Specification<QuestionFolder>, ISingleResultSpecification
{
    public QuestionFoldersWithPermissionsSpecByUserId(DefaultIdType userId, List<Guid> groupTeacherIds, DefaultIdType? parentFolderId)
    {
        Query
        .Include(qf => qf.Permissions)
        .Include(qf => qf.Children)
        .Where(qf => qf.Permissions.Any(qfp => qfp.UserId == userId && qfp.CanView) || qf.Permissions.Any(qfp => groupTeacherIds.Contains((DefaultIdType)qfp.GroupTeacherId) && qfp.CanView))
        .Where(qf => qf.ParentId == parentFolderId)
        .OrderBy(qf => qf.Name);
    }
}