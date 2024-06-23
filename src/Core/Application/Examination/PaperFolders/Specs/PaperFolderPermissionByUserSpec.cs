

using FSH.WebApi.Domain.Examination;
using MediatR;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderPermissionByUserSpec : Specification<PaperFolderPermission>
{
    public PaperFolderPermissionByUserSpec(Guid currentUserId)
    {
        Query.Where(x => x.UserId == currentUserId);
    }
}
