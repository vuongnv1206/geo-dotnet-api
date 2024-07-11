using FSH.WebApi.Application.Class.Dto;


namespace FSH.WebApi.Application.Assignments;
public class SubmissionAssignmentDto
{
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public string Status { get; set; }
    public string? AnswerRaw { get; set; }
    public string? AttachmentPath { get; set; }
    public float? Score { get; set; }
    public string? Comment { get; set; }
    public StudentDto Student { get; set; }
}
