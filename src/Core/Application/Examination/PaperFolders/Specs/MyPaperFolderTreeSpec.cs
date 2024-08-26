

using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class MyPaperFolderTreeSpec : Specification<PaperFolder>
{
    public MyPaperFolderTreeSpec(Guid currentUserId)
    {
        Query
        .Include(x => x.PaperFolderParent)
        .Include(x => x.PaperFolderChildrens)
        .Include(x => x.PaperFolderPermissions)
        .ThenInclude(x => x.GroupTeacher).ThenInclude(x => x.TeacherInGroups)
         .Where(x => x.CreatedBy == currentUserId ||
                    x.PaperFolderPermissions.Any(p => p.UserId == currentUserId ||
                                                      (p.GroupTeacher != null &&
                                                       p.GroupTeacher.TeacherInGroups.Any(g => g.TeacherTeamId == currentUserId))))       //Lấy những cái mình tạo và những cái mình đc share
        .OrderBy(x => x.CreatedOn);
    }
}
