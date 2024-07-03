using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions.Specs;
public class QuestionChildrenFoldersWithPermissionsSpecByUserId : Specification<QuestionFolder>, ISingleResultSpecification
{
    public QuestionChildrenFoldersWithPermissionsSpecByUserId(DefaultIdType userId)
    {
        Query
        .Include(qf => qf.Permissions)
        .Include(qf => qf.Children)
        .Where(qf => qf.Permissions.Any(qfp => qfp.UserId == userId && qfp.CanView) || qf.CreatedBy == userId)
        .Where(qf => qf.ParentId != null)
        .OrderBy(qf => qf.Name);
    }
}