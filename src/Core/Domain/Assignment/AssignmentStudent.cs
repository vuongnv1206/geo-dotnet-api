namespace FSH.WebApi.Domain.Assignment;
public class AssignmentStudent
{
    public Guid AssignmentId { get; private set; }
    public Guid StudentId { get; private set; }
    public string? AttachmentPath { get; private set; }
    public string? Score { get; private set; }

}
