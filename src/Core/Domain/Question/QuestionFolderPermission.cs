namespace FSH.WebApi.Domain.Question;

public class QuestionFolderPermission : AuditableEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid QuestionFolderId { get; private set; }
    public virtual QuestionFolder QuestionFolder { get; private set; } = default!;
    public bool CanView { get; private set; }
    public bool CanAdd { get; private set; }
    public bool CanUpdate { get; private set; }
    public bool CanDelete { get; private set; }

    public QuestionFolderPermission(Guid userId, Guid questionFolderId, bool canView, bool canAdd, bool canUpdate, bool canDelete)
    {
        UserId = userId;
        QuestionFolderId = questionFolderId;
        CanView = canView;
        CanAdd = canAdd;
        CanUpdate = canUpdate;
        CanDelete = canDelete;
    }
}