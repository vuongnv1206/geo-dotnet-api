using FSH.WebApi.Domain.Class;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSH.WebApi.Domain.TeacherGroup;
public class TeacherPermissionInClass : AuditableEntity, IAggregateRoot
{
    public Guid ClassId { get; set; }
    public Guid TeacherTeamId { get; set; }
    public PermissionType PermissionType { get; set; }
    public virtual TeacherTeam TeacherTeam { get; set; }
    [ForeignKey(nameof(ClassId))]
    public virtual Classes? Classroom { get; set; }
}
