namespace FSH.WebApi.Application.TeacherGroup.JoinTeams;
public class InviteJoinTeamEmailModel
{
    public string SenderEmail { get; set; } = default!;
    public string RecipientEmail { get; set; } = default!;
    public string Url { get; set; } = default!;
}

