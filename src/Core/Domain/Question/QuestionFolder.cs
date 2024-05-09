using System.ComponentModel;

namespace FSH.WebApi.Domain.Question;

public class QuestionFolder : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; }
    public Guid? ParentId { get; private set; }
    public virtual QuestionFolder? Parent { get; private set; }

    public virtual List<QuestionFolder> Children { get; private set; } = new();

    public virtual List<QuestionFolderPermission> Permissions { get; private set; } = new();

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
        return Permissions.Any(x => x.UserId == guid && x.CanDelete);
    }

    public bool CanUpdate(DefaultIdType guid)
    {
        if (CreatedBy == guid) return true;
        return Permissions.Any(x => x.UserId == guid && x.CanUpdate);
    }

    public bool CanAdd(DefaultIdType guid)
    {
        if (CreatedBy == guid) return true;
        return Permissions.Any(x => x.UserId == guid && x.CanAdd);
    }

    public bool CanView(DefaultIdType guid)
    {
        if (CreatedBy == guid) return true;
        return Permissions.Any(x => x.UserId == guid && x.CanView);
    }

    public void CopyPermissions(QuestionFolder? parentFolder)
    {
        if (parentFolder is null) return;

        foreach (var permission in parentFolder.Permissions)
        {
            AddPermission(new QuestionFolderPermission(permission.UserId, Id, permission.CanView, permission.CanAdd, permission.CanUpdate, permission.CanDelete));
            AddPermission(new QuestionFolderPermission(parentFolder.CreatedBy, Id, true, true, true, true));
        }
    }
}