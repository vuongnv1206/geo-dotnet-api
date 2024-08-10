namespace FSH.WebApi.Domain.TeacherGroup;
public class JoinGroupTeacherRequest : AuditableEntity, IAggregateRoot
{
    public Guid GroupId { get; set; }
    public Guid TeacherId { get; set; }
    public JoinTeacherGroupStatus Status { get; set; }
    public string? Content { get; set; }
    public virtual GroupTeacher? GroupTeacher { get; set; }
    public virtual TeacherTeam? TeacherTeam { get; set; }

    public JoinGroupTeacherRequest(Guid groupId, Guid teacherId, string? content)
    {
        GroupId = groupId;
        TeacherId = teacherId;
        Status = JoinTeacherGroupStatus.Pending;
        Content = content;
    }
}
