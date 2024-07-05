using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderPermissionByUserOrGroupSpec : Specification<PaperFolderPermission>
{
    public PaperFolderPermissionByUserOrGroupSpec(Guid currentUserId, List<Guid> groupIds)
    {
        Query.Where(p => p.UserId == currentUserId || (groupIds.Contains(p.GroupTeacherId.Value) && p.GroupTeacherId.HasValue));
    }
}
