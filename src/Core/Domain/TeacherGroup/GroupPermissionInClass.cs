namespace FSH.WebApi.Domain.TeacherGroup;
public class GroupPermissionInClass : AuditableEntity, IAggregateRoot
{
    public Guid GroupTeacherId { get; set; }
    public Guid ClassId { get; set; }
    public PermissionType PermissionType { get; set; }
    public virtual GroupTeacher GroupTeacher { get; set; }

    // public virtual Class Class { get; set; }
}
