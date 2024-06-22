

using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Papers;
internal class PaperPermissionByUserSpec : Specification<PaperPermission>
{
    public PaperPermissionByUserSpec(Guid currentUserId)
    {
        Query.Where(x => x.UserId == currentUserId);
    }
}
