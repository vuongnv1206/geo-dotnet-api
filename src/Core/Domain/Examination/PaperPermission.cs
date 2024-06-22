using FSH.WebApi.Domain.TeacherGroup;


namespace FSH.WebApi.Domain.Examination;
public class PaperPermission : AuditableEntity,IAggregateRoot
{
    public Guid UserId { get; set; }
    public Guid PaperId { get; set; }
    public Guid? GroupTeacherId { get; set; }
    public bool CanView { get; private set; }
    public bool CanAdd { get; private set; }
    public bool CanUpdate { get; private set; }
    public bool CanDelete { get; private set; }
    public bool CanShare { get; private set; }
    public virtual Paper Paper { get; set; }
    public virtual GroupTeacher? GroupTeacher { get; set; }

    public PaperPermission(Guid userId, Guid paperId, Guid? groupTeacherId, bool canView, bool canAdd, bool canUpdate, bool canDelete, bool canShare)
    {
        UserId = userId;
        PaperId = paperId;
        CanView = canView;
        CanAdd = canAdd;
        CanUpdate = canUpdate;
        CanDelete = canDelete;
        GroupTeacherId = groupTeacherId;
        CanShare = canShare;
    }

    public void SetPermission(bool canView, bool canAdd, bool canUpdate, bool canDelete,bool canShare)
    {
        CanView = canView;
        CanAdd = canAdd;
        CanUpdate = canUpdate;
        CanDelete = canDelete;
        CanShare = canShare;
    }
}
