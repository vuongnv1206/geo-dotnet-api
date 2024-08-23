using FSH.WebApi.Domain.Examination;
using MediatR;


namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderTreeBySearchSpec : Specification<PaperFolder>
{
    public PaperFolderTreeBySearchSpec(DefaultIdType currentUserId, SearchPaperFolderRequest request)
    {
        Query
        .Include(x => x.PaperFolderParent)
        .Include(x => x.PaperFolderChildrens)
        .Include(x => x.PaperFolderPermissions)
        //.Where(x => x.ParentId == request.ParentId)         //Không where vì để included hết ,sẽ query bên ngoài , gọi là spit query       
        .Where(x => (x.CreatedBy == currentUserId))
        .OrderBy(x => x.CreatedOn);
    }
}
