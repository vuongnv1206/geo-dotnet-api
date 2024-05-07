using FSH.WebApi.Domain.Subjects;

namespace FSH.WebApi.Domain.Assignment;
public class Assignment : AuditableEntity, IAggregateRoot
{

    public string Name { get; private set; } = default!;
    public DateTime? StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public string? AttachmentPath { get; private set; }
    public string? Content { get; private set; }
    public bool CanViewResult { get; private set; }
    public bool RequireLoginToSubmit { get; private set; }
    public Guid SubjectId { get; private set; }
    public virtual Subject Subject { get; private set; } = default!;

    public Assignment()
    {

    }
    public Assignment(string name, DateTime? startTime, DateTime? endTime, string? attachmentPath, string? content, bool canViewResult, bool requireLoginToSubmit, DefaultIdType subjectId)
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
        if (attachmentPath is not null && !AttachmentPath.Equals(attachmentPath))
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

}
