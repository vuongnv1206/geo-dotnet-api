using FSH.WebApi.Domain.TeacherGroup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Domain.Examination;
public class PaperFolderPermission : AuditableEntity, IAggregateRoot
{
    public Guid? UserId { get; set; }
    public Guid FolderId { get; set; }
    public Guid? GroupTeacherId { get; set; }
    public bool CanView { get; private set; }
    public bool CanAdd { get; private set; }
    public bool CanUpdate { get; private set; }
    public bool CanDelete { get; private set; }
    public bool CanShare { get; set; }
    [ForeignKey(nameof(FolderId))]
    public virtual PaperFolder PaperFolder { get; set; }
    [ForeignKey(nameof(GroupTeacherId))]
    public virtual GroupTeacher? GroupTeacher { get; set; }

    public PaperFolderPermission(Guid? userId, Guid folderId, Guid? groupTeacherId, bool canView, bool canAdd, bool canUpdate, bool canDelete,bool canShare)
    {
        UserId = userId;
        FolderId = folderId;
        CanView = canView;
        CanAdd = canAdd;
        CanUpdate = canUpdate;
        CanDelete = canDelete;
        GroupTeacherId = groupTeacherId;
        CanShare = canShare;
    }

    public void SetPermissions(bool canView, bool canAdd, bool canUpdate, bool canDelete,bool canShare)
    {
        CanView = canView;
        CanAdd = canAdd;
        CanUpdate = canUpdate;
        CanDelete = canDelete;
        CanShare = canShare;
    }
}
