using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Domain.Assignment;
public class AssignmentStudent
{
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public SubmitAssignmentStatus Status { get; set; }
    public string? AnswerRaw { get; set; }
    public string? AttachmentPath { get; set; }
    public float? Score { get; set; }
    public string? Comment { get; set; }
    public virtual Student Student { get; set; }
    public virtual Assignment Assignment { get; set; }

    public AssignmentStudent()
    {
    }

    public AssignmentStudent(Guid assignmentId, Guid studentId)
    {
        AssignmentId = assignmentId;
        StudentId = studentId;
        Status = SubmitAssignmentStatus.Doing;
    }
}
