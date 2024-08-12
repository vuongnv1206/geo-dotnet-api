namespace FSH.WebApi.Domain.TeacherGroup;
public class InviteJoinTeacherTeam : AuditableEntity, IAggregateRoot
{
    public string RecipientEmail { get; set; }
    public string SenderEmail { get; set; }
    public bool IsRegistered { get; set; }
}
