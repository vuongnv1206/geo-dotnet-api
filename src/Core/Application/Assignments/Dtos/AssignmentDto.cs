namespace FSH.WebApi.Application.Assignments.Dtos;

public class AssignmentDto : IDto
{
public Guid Id { get; set; }
public string Name { get; set; } = default!;
public DateTime? StartTime { get; set; }
public DateTime? EndTime { get; set; }
public string? AttachmentPath { get; set; }
public string? Content { get; set; }
public bool CanViewResult { get; set; }
public bool RequireLoginToSubmit { get; set; }
public Guid SubjectId { get; set; }
public string SubjectName { get; set; } = default!;

}