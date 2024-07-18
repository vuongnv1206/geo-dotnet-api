using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Domain.Question;

public class QuestionFolder : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; }
    public Guid? ParentId { get; private set; }
    public virtual QuestionFolder? Parent { get; private set; }

    public virtual ICollection<QuestionFolder> Children { get; private set; } = new List<QuestionFolder>();

    public virtual ICollection<QuestionFolderPermission> Permissions { get; private set; } = new List<QuestionFolderPermission>();

    public QuestionFolder(string name, Guid? parentId)
    {
        Name = name;
        ParentId = parentId;
    }

    public void AddChild(QuestionFolder questionFolder)
    {
        Children.Add(questionFolder);
    }

    public void AddPermission(QuestionFolderPermission permission)
    {
        Permissions.Add(permission);
    }

    public QuestionFolder Update(string name, Guid? parentId)
    {
        Name = name;
        ParentId = parentId;
        return this;
    }

    public bool CanDelete(DefaultIdType guid)
    {
        if (CreatedBy == guid) return true;
        if (Permissions == null) return false;
        return Permissions.Any(x => x.UserId == guid && x.CanDelete);
    }

    public bool CanUpdate(DefaultIdType guid)
    {
        if (CreatedBy == guid) return true;
        if (Permissions == null) return false;
        return Permissions.Any(x => x.UserId == guid && x.CanUpdate);
    }

    public bool CanAdd(DefaultIdType guid)
    {
        if (CreatedBy == guid) return true;
        if (Permissions == null) return false;
        return Permissions.Any(x => x.UserId == guid && x.CanAdd);
    }

    public bool CanView(DefaultIdType guid)
    {
        if (CreatedBy == guid) return true;
        if (Permissions == null) return false;
        return Permissions.Any(x => x.UserId == guid && x.CanView);
    }

    public bool CanShare(DefaultIdType guid)
    {
        if (CreatedBy == guid) return true;
        if (Permissions == null) return false;
        return Permissions.Any(x => x.UserId == guid && x.CanShare);
    }

    public void CopyPermissions(QuestionFolder? parentFolder)
    {
        if (parentFolder is null) return;

        foreach (var permission in parentFolder.Permissions)
        {
            AddPermission(new QuestionFolderPermission(permission.UserId, permission.GroupTeacherId, Id, permission.CanView, permission.CanAdd, permission.CanUpdate, permission.CanDelete, permission.CanShare));
        }
    }

    public IEnumerable<QuestionFolder> GetAllDescendants()
    {
        var descendants = new List<QuestionFolder>();
        AddDescendants(this, descendants);
        return descendants;
    }

    private void AddDescendants(QuestionFolder folder, List<QuestionFolder> descendants)
    {
        if (folder.Children != null && folder.Children.Any())
        {
            foreach (var child in folder.Children)
            {
                descendants.Add(child);
                AddDescendants(child, descendants);
            }
        }
    }

    public void ChildQuestionFolderIds(ICollection<QuestionFolder> childs, List<Guid> ids)
    {
        if (childs == null)
            childs = this.Children;

        foreach (QuestionFolder questionFolder in childs)
        {
            ids.Add(questionFolder.Id);
            ChildQuestionFolderIds(questionFolder.Children, ids);
        }
    }
}