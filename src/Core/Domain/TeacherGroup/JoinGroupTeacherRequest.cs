using System.ComponentModel.DataAnnotations.Schema;

namespace FSH.WebApi.Domain.TeacherGroup;
public class JoinGroupTeacherRequest : AuditableEntity, IAggregateRoot
{
    public Guid GroupId { get; set; }
    public Guid TeacherId { get; set; }
    public Guid ReceiverId { get; set; }
    public JoinTeacherGroupStatus Status { get; set; }
    public string? Content { get; set; }
    [ForeignKey(nameof(GroupId))]
    public virtual GroupTeacher? GroupTeacher { get; set; }
    [ForeignKey(nameof(TeacherId))]
    public virtual TeacherTeam? TeacherTeam { get; set; }

    public JoinGroupTeacherRequest(Guid groupId, Guid teacherId, Guid receiverId, string? content)
    {
        GroupId = groupId;
        TeacherId = teacherId;
        ReceiverId = receiverId;
        Status = JoinTeacherGroupStatus.Pending;
        Content = content;
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
