namespace FSH.WebApi.Domain.Question;

public class QuestionFolderPermission : AuditableEntity, IAggregateRoot
{
    public DefaultIdType? UserId { get; private set; } = default!;
    public DefaultIdType? GroupTeacherId { get; private set; } = default!;
    public DefaultIdType QuestionFolderId { get; private set; }
    public virtual QuestionFolder QuestionFolder { get; private set; } = default!;
    public bool CanView { get; private set; }
    public bool CanAdd { get; private set; }
    public bool CanUpdate { get; private set; }
    public bool CanDelete { get; private set; }

    public QuestionFolderPermission(DefaultIdType? userId, DefaultIdType? groupTeacherId, DefaultIdType questionFolderId, bool canView, bool canAdd, bool canUpdate, bool canDelete)
    {
        UserId = userId;
        GroupTeacherId = groupTeacherId;
        QuestionFolderId = questionFolderId;
        CanView = canView;
        CanAdd = canAdd;
        CanUpdate = canUpdate;
        CanDelete = canDelete;
    }

    public QuestionFolderPermission(DefaultIdType? userId, DefaultIdType questionFolderId, bool canView, bool canAdd, bool canUpdate, bool canDelete)
    {
        UserId = userId;
        QuestionFolderId = questionFolderId;
        CanView = canView;
        CanAdd = canAdd;
        CanUpdate = canUpdate;
        CanDelete = canDelete;
    }

    public QuestionFolderPermission(DefaultIdType id, DefaultIdType userId, DefaultIdType groupTeacherId)
    {
        Id = id;
        UserId = userId;
        GroupTeacherId = groupTeacherId;
    }

    public QuestionFolderPermission(DefaultIdType id, DefaultIdType userId)
    {
        Id = id;
        UserId = userId;
    }

    public void SetPermissions(bool canView, bool canAdd, bool canUpdate, bool canDelete)
    {
        CanView = canView;
        CanAdd = canAdd;
        CanUpdate = canUpdate;
        CanDelete = canDelete;
    }
}