

namespace FSH.WebApi.Domain.TeacherGroup;
public class TeacherPermissionInClass : AuditableEntity,IAggregateRoot
{
    public Guid ClassId { get; set; }
    public Guid TeacherTeamId { get; set; }
    public PermissionType PermissionType { get; set; }
    public virtual TeacherTeam TeacherTeam { get; set; }
    //public virtual Class Class { get; set; }


}
