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

    public void MarkScoreForSubmission(Guid studentId, float score, string? comment)
    {
        var submission = AssignmentStudents.FirstOrDefault(x => x.StudentId == studentId);
        if (submission is not null)
        {
            submission.Score = score;
            submission.Comment = comment;
            submission.Status = SubmitAssignmentStatus.Marked;
        }
    }

    public void SubmitAssignment(Guid studentId, string? answerRaw, string? attachmentPath)
    {
        var assignmentStudent = AssignmentStudents.Where(x => x.Student.StId == studentId).FirstOrDefault();
        if (assignmentStudent is not null)
        {
            assignmentStudent.AnswerRaw = answerRaw;
            assignmentStudent.AttachmentPath = attachmentPath;
            assignmentStudent.Status = SubmitAssignmentStatus.Submitted;
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

    public void RemoveSubmissionFromClass(List<Guid> studentIds)
    {
        AssignmentStudents.RemoveAll(x => studentIds.Contains(x.StudentId));
    }

    public void RemoveAssignmentOfStudent(Guid studentId)
    {
        AssignmentStudents.RemoveAll(x => x.StudentId == studentId);
    }

    public void UpdateStatusSubmitAssignment(SubmitAssignmentStatus statusFrom, SubmitAssignmentStatus statusTo)
    {
        AssignmentStudents.Where(s => s.Status == statusFrom)
                   .ToList()
                   .ForEach(s => s.Status = statusTo);
    }

    //write func to  remove score and comment of assignmentStudent ,and update all status to Doing
    public void ExtendTimeAssignment()
    {
        AssignmentStudents.ForEach(s =>
        {
            s.Score = null;
            s.Comment = null;
            s.Status = SubmitAssignmentStatus.Doing;
        });
    }


}