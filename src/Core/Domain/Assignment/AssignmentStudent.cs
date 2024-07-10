using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Domain.Assignment;
public class AssignmentStudent
{
    public Guid AssignmentId { get; private set; }
    public Guid StudentId { get; private set; }
    public SubmitAssignmentStatus Status { get; set; }
    public string? AnswerRaw { get; set; }
    public string? AttachmentPath { get; private set; }
    public string? Score { get; private set; }
    public virtual Student Student { get; set; }
    public virtual Assignment Assignment { get; set; }

    public AssignmentStudent()
    {

    }

  
}
