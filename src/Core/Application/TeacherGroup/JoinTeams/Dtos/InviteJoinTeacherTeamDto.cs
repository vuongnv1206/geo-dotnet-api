
namespace FSH.WebApi.Application.TeacherGroup.JoinTeams;
public class InviteJoinTeacherTeamDto
{
    public Guid Id { get; set; }
    public string RecipientEmail { get; set; }
    public string SenderEmail { get; set; }
    public bool IsRegistered { get; set; }
    public InvitationStatus Status { get; set; }
    public DateTime? CreateOn { get; set; }
}

public enum InvitationStatus
{
    NotRequest = 0,
    Requested = 1,
    BeRejected = 2,
    BeAccepted = 3,
}
