
using FSH.WebApi.Domain.Examination;


namespace FSH.WebApi.Application.Examination.Papers;
internal class PaperPermissionByUserOrGroupSpec : Specification<PaperPermission>
{
    public PaperPermissionByUserOrGroupSpec(Guid currentUserId, List<Guid> groupIds)
    {
        Query.Where(p => p.UserId == currentUserId || (groupIds.Contains(p.GroupTeacherId.Value) && p.GroupTeacherId.HasValue))
            .Where(p => p.CreatedBy != currentUserId)
            ;

    }
}
