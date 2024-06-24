using FSH.WebApi.Domain.Class;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSH.WebApi.Domain.TeacherGroup;
public class GroupPermissionInClass : AuditableEntity, IAggregateRoot
{
    public Guid GroupTeacherId { get; set; }
    public Guid ClassId { get; set; }
    public PermissionType PermissionType { get; set; }
    public virtual GroupTeacher GroupTeacher { get; set; }
    [ForeignKey(nameof(ClassId))]
    public virtual Classes? Classroom { get; set; }
}
