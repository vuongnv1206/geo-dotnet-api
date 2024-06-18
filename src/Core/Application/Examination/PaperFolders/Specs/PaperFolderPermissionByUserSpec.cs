

using FSH.WebApi.Domain.Examination;
using MediatR;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderPermissionByUserSpec : EntitiesByPaginationFilterSpec<PaperFolderPermission>
{
    public PaperFolderPermissionByUserSpec(Guid currentUserId,SearchSharedPaperFolderRequest request)
        : base(request)
    {
        
    }
}
