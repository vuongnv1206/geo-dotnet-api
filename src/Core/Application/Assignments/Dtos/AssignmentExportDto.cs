namespace FSH.WebApi.Application.Assignments.Dtos;
public class AssignmentExportDto : IDto
{
    public string Name { get; set; } = default!;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? AttachmentPath { get; set; }
    public string? Content { get; set; }
    public bool CanViewResult { get; set; }
    public bool RequireLoginToSubmit { get; set; }
    public string SubjectName { get; set; } = default!;

}