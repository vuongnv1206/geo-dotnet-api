using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Subjects;
namespace FSH.WebApi.Domain.Assignment;
public class Assignment : AuditableEntity, IAggregateRoot
{

    public string Name { get; set; } = default!;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Attachment { get; set; }
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

    public Assignment(string name, DateTime? startTime, DateTime? endTime, string? attachment, string? content, bool canViewResult, bool requireLoginToSubmit, Guid subjectId)
    {
        Name = name;
        StartTime = startTime;
        EndTime = endTime;
        Attachment = attachment;
        Content = content;
        CanViewResult = canViewResult;
        RequireLoginToSubmit = requireLoginToSubmit;
        SubjectId = subjectId;
    }

    public Assignment Update(string? name, DateTime? startTime, DateTime? endTime, string? attachment, string? content, bool? canViewResult, bool? requireLoginToSubmit, Guid? subjectId)
    {
        if (name is not null && !Name.Equals(name))
            Name = name;
        if (startTime.HasValue && StartTime != startTime)
            StartTime = startTime;
        if (endTime.HasValue && EndTime != endTime)
            EndTime = endTime;
        if (attachment is not null)
            Attachment = attachment;
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
        Attachment = string.Empty;
        return this;
    }

    public void AssignAssignmentToClass(Guid classId)
    {
        AssignmentClasses.Add(new AssignmentClass
        {
            AssignmentId = this.Id,
            ClassesId = classId
        });
    }

    public void MarkScoreForSubmission(Guid studentId, float score,string? comment)
    {
        var submission = AssignmentStudents.FirstOrDefault(x => x.StudentId == studentId);
        if (submission is not null)
        {
            submission.Score = score;
            submission.Comment = comment;
        }
    }

    public void SubmitAssignment(Guid studentId, string? answerRaw,string? attachmentPath)
    {
        var assignmentStudent = AssignmentStudents.FirstOrDefault(x => x.StudentId == studentId);
        if (assignmentStudent is not null)
        {
            assignmentStudent.AnswerRaw = answerRaw;
            assignmentStudent.AttachmentPath = attachmentPath;
            assignmentStudent.Status = SubmitAssignmentStatus.Submmitted;
        }
    }

    public void AssignAssignmentToStudent(Guid studentId)
    {
        if (!AssignmentStudents.Any(a => a.StudentId == studentId))
        {
            AssignmentStudents.Add(new AssignmentStudent
            {
                AssignmentId = this.Id,
                StudentId = studentId
            });
        }
    }

    public void AssignAssignmentToStudents(List<Guid> studentIds)
    {
        foreach (var studentId in studentIds)
        {
            if (!AssignmentStudents.Any(a => a.StudentId == studentId))
            {
                AssignmentStudents.Add(new AssignmentStudent
                {
                    AssignmentId = this.Id,
                    StudentId = studentId
                });
            }
        }
    }

    public void RemoveAssignmentFromClass(Guid classId)
    {
        AssignmentStudents.RemoveAll(x => AssignmentClasses.Any(ac => ac.ClassesId == classId && ac.AssignmentId == x.AssignmentId));
    }


 
    public void RemoveAssignmentOfStudent(Guid studentId)
    {
        AssignmentStudents.RemoveAll(x => x.StudentId == studentId);
    }


    public void UpdateStatusSubmitAssignment(SubmitAssignmentStatus status)
    {
        AssignmentStudents.Where(s => s.Status == SubmitAssignmentStatus.Doing)
                   .ToList()
                   .ForEach(s => s.Status = status);
    }
    
}