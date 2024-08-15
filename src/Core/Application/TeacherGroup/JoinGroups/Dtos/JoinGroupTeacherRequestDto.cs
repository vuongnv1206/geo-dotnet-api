using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinGroups;
public class JoinGroupTeacherRequestDto : IDto
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public string? GroupName { get; set; }
    public Guid TeacherId { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Guid ReceiverId { get; set; }
    public string? ReceiverEmail { get; set; }
    public JoinTeacherGroupStatus Status { get; set; }
    public string? Content { get; set; }
    public DateTime? CreateOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}
