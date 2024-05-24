using FSH.WebApi.Domain.Question;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Domain.Examination;
public class PaperFolder : AuditableEntity, IAggregateRoot
{
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? SubjectId { get; set; }
    //public virtual Subject Subject { get; set; }
    [ForeignKey(nameof(ParentId))]
    public virtual PaperFolder? PaperFolderParent { get; set; }
    public virtual List<PaperFolder>? PaperFolderChildrens { get; set; } = new();
    public virtual List<PaperFolderPermission> PaperFolderPermissions { get; set; } = new();
    public virtual List<Paper> Papers { get; set; }

    public PaperFolder(string name, Guid? parentId, Guid? subjectId)
    {
        Name = name;
        ParentId = parentId;
        SubjectId = subjectId;
    }

    public PaperFolder Update(string name, Guid? parentId, Guid? subjectId)
    {
        Name = name;
        ParentId = parentId;
        SubjectId = subjectId;

        return this;
    }

    public void AddPermission(PaperFolderPermission permission)
    {
        PaperFolderPermissions.Add(permission);
    }

    public bool CanDelete(Guid userId)
    {
        if (CreatedBy == userId) return true;
        return PaperFolderPermissions.Any(x => x.UserId == userId && x.CanDelete);
    }

    public bool CanUpdate(Guid userId)
    {
        if (CreatedBy == userId) return true;
        return PaperFolderPermissions.Any(x => x.UserId == userId && x.CanUpdate);
    }

    public bool CanAdd(Guid userId)
    {
        if (CreatedBy == userId) return true;
        return PaperFolderPermissions.Any(x => x.UserId == userId && x.CanAdd);
    }

    public bool CanView(Guid userId)
    {
        if (CreatedBy == userId) return true;
        return PaperFolderPermissions.Any(x => x.UserId == userId && x.CanView);
    }

    public void CopyPermissions(PaperFolder? paperFolderParent)
    {
        if (paperFolderParent is null) return;

        foreach (var permission in paperFolderParent.PaperFolderPermissions)
        {
            AddPermission(new PaperFolderPermission(permission.UserId, Id, permission.GroupTeacherId, permission.CanView, permission.CanAdd, permission.CanUpdate, permission.CanDelete));
            AddPermission(new PaperFolderPermission(paperFolderParent.CreatedBy, Id, permission.GroupTeacherId, true, true, true, true));
        }
    }
}
