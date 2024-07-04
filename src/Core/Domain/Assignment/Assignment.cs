using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Subjects;
namespace FSH.WebApi.Domain.Assignment;
public class Assignment : AuditableEntity, IAggregateRoot
{

    public string Name { get; set; } = default!;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? AttachmentPath { get; set; }
    public string? Content { get; set; }
    public bool CanViewResult { get; set; }
    public bool RequireLoginToSubmit { get; set; }
    public Guid? SubjectId { get; set; }
    public virtual Subject Subject { get; set; } = default!;
    public virtual List<AssignmentClass> AssignmentClasses { get; set; } = new();
    public virtual List<AssignmentStudent> AssignmentStudents { get; set; } = new();

    public Assignment()
    {
    }

    public Assignment(string name, DateTime? startTime, DateTime? endTime, string? attachmentPath, string? content, bool canViewResult, bool requireLoginToSubmit, Guid subjectId)
    {
        Name = name;
        StartTime = startTime;
        EndTime = endTime;
        AttachmentPath = attachmentPath;
        Content = content;
        CanViewResult = canViewResult;
        RequireLoginToSubmit = requireLoginToSubmit;
        SubjectId = subjectId;
    }

    public Assignment Update(string? name, DateTime? startTime, DateTime? endTime, string? attachmentPath, string? content, bool? canViewResult, bool? requireLoginToSubmit, Guid? subjectId)
    {
        if (name is not null && !Name.Equals(name))
            Name = name;
        if (startTime.HasValue && StartTime != startTime)
            StartTime = startTime;
        if (endTime.HasValue && EndTime != endTime)
            EndTime = endTime;
        if (attachmentPath is not null)
            AttachmentPath = attachmentPath;
        if (content is not null && !Content.Equals(content))
            Content = content;
        if (canViewResult.HasValue && CanViewResult != canViewResult)
            CanViewResult = canViewResult.Value;
        if (requireLoginToSubmit.HasValue && RequireLoginToSubmit != requireLoginToSubmit)
            RequireLoginToSubmit = requireLoginToSubmit.Value;
        if (subjectId.HasValue && SubjectId != subjectId)
            SubjectId = subjectId.Value;

        return this;
    }

    public Assignment ClearAttachmentPath()
    {
        AttachmentPath = string.Empty;
        return this;
    }

    //public void AssignAssignmentToClass(AssignmentClass assignmentClass)
    //{
    //    AssignmentClasses.Add(assignmentClass);
    //}

    public void UpdateAssignmentFromClass(List<AssignmentClass> aClass)
    {
        AssignmentClasses.RemoveAll(ac => !aClass.Any(c => c.AssignmentId == ac.AssignmentId));
        foreach (var ac in aClass)
        {
            AssignmentClasses.Add(new AssignmentClass
            {
                AssignmentId = this.Id,
                ClassesId = ac.ClassesId
            });
        }
    }
}