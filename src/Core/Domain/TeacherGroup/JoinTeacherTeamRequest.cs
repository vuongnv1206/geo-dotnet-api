namespace FSH.WebApi.Domain.TeacherGroup;
public class JoinTeacherTeamRequest : AuditableEntity, IAggregateRoot
{
    public Guid AdminTeamId { get; set; }
    public JoinTeacherGroupStatus Status { get; set; }
    public string? Content { get; set; }
    public string SenderEmail { get; set; }
    public Guid? InvitationId { get; set; }

    public JoinTeacherTeamRequest(Guid adminTeamId, string? content, string senderEmail, Guid? invitationId)
    {
        AdminTeamId = adminTeamId;
        Content = content;
        Status = JoinTeacherGroupStatus.Pending;
        SenderEmail = senderEmail;
        InvitationId = invitationId;
    }

    public void AcceptRequest()
    {
        Status = JoinTeacherGroupStatus.Accepted;
    }

    public void RejectRequest()
    {
        Status = JoinTeacherGroupStatus.Rejected;
    }

    public void CancelRequest()
    {
        Status = JoinTeacherGroupStatus.Cancel;
    }
}
