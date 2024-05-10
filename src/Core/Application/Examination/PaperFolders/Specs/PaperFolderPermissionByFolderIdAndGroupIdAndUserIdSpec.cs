using FSH.WebApi.Domain.Examination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.PaperFolders.Specs;
public class PaperFolderPermissionByFolderIdAndGroupIdAndUserIdSpec : Specification<PaperFolderPermission>, ISingleResultSpecification
{
    public PaperFolderPermissionByFolderIdAndGroupIdAndUserIdSpec(Guid folderId, Guid userId, Guid? groupId) =>
        Query
        .Where(x => x.FolderId == folderId && x.UserId == userId && (!groupId.HasValue || x.GroupTeacherId == groupId));
}
