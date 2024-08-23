using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinTeams;
public class JoinTeacherTeamRequestDto : IDto
{
    public Guid Id { get; set; }
    public Guid AdminTeamId { get; set; }
    public string? AdminTeamEmail { get; set; }
    public JoinTeacherGroupStatus Status { get; set; }
    public string? Content { get; set; }
    public Guid CreateBy { get; set; }
    public string? SenderEmail { get; set; }
    public string? SenderFullName { get; set; }
    public DateTime? CreateOn { get; set; }
    public DateTime? LastModifiedOn { get; set;}
}
