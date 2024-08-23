using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSH.WebApi.Domain.Examination;
public class PaperFolder : AuditableEntity, IAggregateRoot
{
    public string Name { get; set; }
    public Guid? ParentId { get; set; }

    [ForeignKey(nameof(ParentId))]
    public virtual PaperFolder? PaperFolderParent { get; set; }
    public virtual List<PaperFolder>? PaperFolderChildrens { get; set; } = new();
    public virtual List<PaperFolderPermission> PaperFolderPermissions { get; set; } = new();
    public virtual List<Paper> Papers { get; set; } = new();
    public PaperFolder(string name, Guid? parentId)
    {
        Name = name;
        ParentId = parentId;
    }

    public PaperFolder()
    {
    }

    public PaperFolder Update(string name, Guid? parentId)
    {
        Name = name;
        ParentId = parentId;
        return this;
    }

    public void RemoveAllPapers()
    {
        Papers.Clear();
    }

    public void RemoveChildFolders()
    {
        foreach (var child in PaperFolderChildrens.ToList())
        {
            child.RemoveAllPapers();
            child.RemoveChildFolders();
            PaperFolderChildrens.Remove(child);
        }
    }

    public string GetFolderPath()
    {
        var path = new List<string>();
        var currentFolder = this;

        while (currentFolder != null)
        {
            path.Add(currentFolder.Name); 
            currentFolder = currentFolder.PaperFolderParent; 
        }
        path.Reverse();
        return string.Join("/", path);
    }

    public List<PaperFolder> ListParents()
    {
        List<PaperFolder> list = new List<PaperFolder>();
        list.Add(this);
        var parent = this.PaperFolderParent;
        while (parent != null)
        {
            list.Add(parent);
            parent = parent.PaperFolderParent;

        }
        list.Reverse();
        return list;
    }


    public List<PaperFolder> ListAccessibleParents(IEnumerable<Guid> accessibleFolderIds)
    {
        List<PaperFolder> accessibleParents = new List<PaperFolder>();
        accessibleParents.Add(this);
        var parent = this.PaperFolderParent;

        while (parent != null)
        {
            if (accessibleFolderIds.Contains(parent.Id))
            {
                accessibleParents.Add(parent);
            }
            parent = parent.PaperFolderParent;
        }

        accessibleParents.Reverse(); // Đảo ngược danh sách để có thứ tự từ cha đến con
        return accessibleParents;
    }


    public void ChildPaperFolderIds(ICollection<PaperFolder> childs, List<Guid> ids)
    {
        if (childs == null)
            childs = this.PaperFolderChildrens;

        foreach (PaperFolder paperFolder in childs)
        {
            ids.Add(paperFolder.Id);
            ChildPaperFolderIds(paperFolder.PaperFolderChildrens, ids);
        }
    }
    public bool HasPermission(Guid userId)
    {
        // Kiểm tra xem có bất kỳ quyền nào được gán cho người dùng này không
        return PaperFolderPermissions.Any(permission => permission.UserId == userId);
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
    public bool CanShare(Guid userId)
    {
        if (CreatedBy == userId) return true;
        return PaperFolderPermissions.Any(x => x.UserId == userId && x.CanShare);
    }

    public void UpdatePermissions(List<PaperFolderPermission> permissions)
    {
        foreach (var permission in permissions)
        {
            var existingPermission = PaperFolderPermissions.FirstOrDefault(a => a.Id == permission.Id && a.Id != Guid.Empty);
            if (existingPermission != null)
            {
                existingPermission.SetPermissions(permission.CanView, permission.CanAdd, permission.CanUpdate, permission.CanDelete, permission.CanShare);
            }
            else
            {
                AddPermission(permission);
            }
        }
    }

    public void CopyPermissions(PaperFolder? paperFolderParent)
    {
        if (paperFolderParent is null) return;

        foreach (var permission in paperFolderParent.PaperFolderPermissions)
        {
            //AddPermission(new PaperFolderPermission(permission.UserId, Id, permission.GroupTeacherId, permission.CanView, permission.CanAdd, permission.CanUpdate, permission.CanDelete,permission.CanShare));
            //AddPermission(new PaperFolderPermission(paperFolderParent.CreatedBy, Id, permission.GroupTeacherId, true, true, true, true,true));

            if (!PaperFolderPermissions.Any(p => p.UserId == permission.UserId && p.GroupTeacherId == permission.GroupTeacherId))
            {
                AddPermission(new PaperFolderPermission(permission.UserId, Id, permission.GroupTeacherId, permission.CanView, permission.CanAdd, permission.CanUpdate, permission.CanDelete, permission.CanShare));
            }

            // Thêm permission cho owner của thư mục cha, nhưng chỉ khi nó chưa tồn tại
            if (!PaperFolderPermissions.Any(p => p.UserId == paperFolderParent.CreatedBy && p.GroupTeacherId == permission.GroupTeacherId))
            {
                AddPermission(new PaperFolderPermission(paperFolderParent.CreatedBy, Id, permission.GroupTeacherId, true, true, true, true, true));
            }
        }
    }

    public int CountPapers()
    {
        int count = Papers.Count;
        foreach (var child in PaperFolderChildrens)
        {
            count += child.CountPapers();
        }
        return count;
    }

    



}
