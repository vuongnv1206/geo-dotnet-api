using FSH.WebApi.Application.Subjects;

namespace FSH.WebApi.Application.Assignments.Dtos;
public class AssignmentDetailsDto : IDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Attachment { get; set; }
    public string? Content { get; set; }
    public bool CanViewResult { get; set; }
    public bool RequireLoginToSubmit { get; set; }
    public SubjectDto Subject { get; set; } = default!;
    public List<Guid>? ClassesId { get; set; }

}