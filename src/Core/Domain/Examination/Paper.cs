using FSH.WebApi.Domain.Examination.Enums;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Subjects;
using System.Linq;

namespace FSH.WebApi.Domain.Examination;
public class Paper : AuditableEntity, IAggregateRoot
{
    // General config
    public string ExamName { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public float? Duration { get; set; }
    public PaperType Type { get; set; }
    public bool IsPublish { get; set; }
    public string? Content { get; set; }
    public string? Description { get; set; }
    public PaperStatus Status { get; set; }

    // Security
    public bool Shuffle { get; set; }
    public ShowResult ShowMarkResult { get; set; }
    public ShowQuestionAnswer ShowQuestionAnswer { get; set; }
    public string? Password { get; set; }
    public string ExamCode { get; set; }
    public int? NumberAttempt { get; set; }
    public PaperShareType ShareType { get; set; }
    public string? PublicIpAllowed { get; set; }
    public string? LocalIpAllowed { get; set; }

    // Navigation
    public Guid? PaperLabelId { get; set; }
    public Guid? PaperFolderId { get; set; }
    public Guid? SubjectId { get; set; }
    public virtual Subject? Subject { get; set; }
    public virtual PaperLabel? PaperLabel { get; set; }
    public virtual PaperFolder? PaperFolder { get; set; }
    public virtual List<PaperQuestion> PaperQuestions { get; set; } = new();
    public virtual List<SubmitPaper> SubmitPapers { get; set; } = new();
    public virtual List<PaperAccess> PaperAccesses { get; set; } = new();
    public virtual List<PaperPermission> PaperPermissions { get; set; } = new();
    public Paper(
        string examName,
        PaperStatus status,
        PaperType type,
        string? content,
        string? description,
        string? password,
        Guid? paperFolderId,
        Guid? paperLabelId,
        Guid? subjectId)
    {
        ExamName = examName;
        Status = status;
        Type = type;
        Content = content;
        Description = description;
        PaperFolderId = paperFolderId;
        PaperLabelId = paperLabelId;
        SubjectId = subjectId;
        Password = password;
        ExamCode = examName;
    }
    public void UpdateTime(DateTime? startTime, DateTime? endTime, float? duration)
    {
        StartTime = startTime;
        EndTime = endTime;
        Duration = duration;
    }

    public void AddQuestions(List<PaperQuestion> questions)
    {
        foreach (var q in questions)
        {
            if (PaperQuestions.Any(x => x.QuestionId == q.QuestionId))
                continue;

            PaperQuestions.Add(new PaperQuestion
            {
                QuestionId = q.QuestionId,
                PaperId = Id,
                Mark = q.Mark,
                RawIndex = q.RawIndex
            });
        }
    }

    public void AddQuestion(PaperQuestion question)
    {
        PaperQuestions.Add(question);
    }

    public void RemoveQuestion(Guid questionId)
    {
        PaperQuestions.RemoveAll(x => x.QuestionId == questionId);
    }

    public void UpdateQuestions(List<PaperQuestion> questions)
    {
        PaperQuestions.RemoveAll(pq => !questions.Any(q => q.QuestionId == pq.QuestionId));
        foreach (var q in questions)
        {
            var existingPaperQuestion = PaperQuestions.FirstOrDefault(pq => pq.QuestionId == q.QuestionId);
            if (existingPaperQuestion != null)
            {
                existingPaperQuestion.Mark = q.Mark;
                existingPaperQuestion.RawIndex = q.RawIndex;
            }
            else
            {
                PaperQuestions.Add(new PaperQuestion
                {
                    QuestionId = q.QuestionId,
                    PaperId = this.Id,
                    Mark = q.Mark,
                    RawIndex = q.RawIndex
                });
            }
        }
    }

    #region update constructor
    public Paper Update(
        string examName,
        PaperStatus status,
        DateTime? startTime,
        DateTime? endTime,
        float? duration,
        bool shuffle,
        ShowResult showMarkResult,
        ShowQuestionAnswer showQuestionAnswer,
        PaperType type,
        bool isPublish,
        string? content,
        string? description,
        string? password,
        int? numberAttempt,
        PaperShareType shareType,
        Guid? paperFolderId,
        Guid? paperLabelId,
        Guid? subjectId,
        string? publicIpAllowed,
        string? localIpAllowed)
    {
        ExamName = examName;
        Status = status;
        StartTime = startTime;
        EndTime = endTime;
        Duration = duration;
        Shuffle = shuffle;
        ShowMarkResult = showMarkResult;
        ShowQuestionAnswer = showQuestionAnswer;
        Type = type;
        NumberAttempt = numberAttempt;
        ShareType = shareType;
        IsPublish = isPublish;
        Content = content;
        Description = description;
        Password = password;
        PaperLabelId = paperLabelId;
        SubjectId = subjectId;
        PaperFolderId = paperFolderId;
        PublicIpAllowed = publicIpAllowed;
        LocalIpAllowed = localIpAllowed;
        return this;
    }
    #endregion

    public void UpdatePaperAccesses(PaperShareType shareType, List<PaperAccess> newPaperAccesses)
    {
        if (shareType == PaperShareType.AssignToStudent)
        {
            // Xác định các PaperAccess mới cần thêm
            var toAdd = newPaperAccesses.Where(npa => !PaperAccesses.Any(pa => pa.UserId == npa.UserId)).ToList();
            // Xóa các PaperAccess không có trong danh sách mới
            PaperAccesses.RemoveAll(pa => !newPaperAccesses.Any(npa => npa.UserId == pa.UserId));
            // Thêm các PaperAccess mới
            PaperAccesses.AddRange(toAdd);
        }
        else if (shareType == PaperShareType.AssignToClass)
        {

            var toAdd = newPaperAccesses.Where(npa => !PaperAccesses.Any(pa => pa.ClassId == npa.ClassId)).ToList();
            PaperAccesses.RemoveAll(pa => !newPaperAccesses.Any(npa => npa.ClassId == pa.ClassId));
            PaperAccesses.AddRange(toAdd);
        }
    }

    public void UpdatePermissions(List<PaperPermission> permissions)
    {
        PaperPermissions.RemoveAll(a => !permissions.Any(x => x.Id == a.Id));

        foreach (var permission in permissions)
        {
            var existingPermission = PaperPermissions.FirstOrDefault(a => a.Id == permission.Id && a.Id != Guid.Empty);
            if (existingPermission != null)
            {
                existingPermission.SetPermission(permission.CanView, permission.CanAdd, permission.CanUpdate, permission.CanDelete, permission.CanShare);
            }
            else
            {
                AddPermission(permission);
            }
        }
    }

    public bool HasPermission(Guid userId)
    {
        // Kiểm tra xem có bất kỳ quyền nào được gán cho người dùng này không
        return PaperPermissions.Any(permission => permission.UserId == userId);
    }

    public void AddPermission(PaperPermission permission)
    {
        PaperPermissions.Add(permission);
    }

    public bool CanDelete(Guid userId)
    {
        if (CreatedBy == userId) return true;
        return PaperPermissions.Any(x => x.UserId == userId && x.CanDelete);
    }

    public bool CanUpdate(Guid userId)
    {
        if (CreatedBy == userId) return true;
        return PaperPermissions.Any(x => x.UserId == userId && x.CanUpdate);
    }

    public bool CanAdd(Guid userId)
    {
        if (CreatedBy == userId) return true;
        return PaperPermissions.Any(x => x.UserId == userId && x.CanAdd);
    }

    public bool CanView(Guid userId)
    {
        if (CreatedBy == userId) return true;
        return PaperPermissions.Any(x => x.UserId == userId && x.CanView);
    }



    public bool CanShare(Guid userId)
    {
        if (CreatedBy == userId) return true;
        return PaperPermissions.Any(x => x.UserId == userId && x.CanShare);
    }

    // lấy ra số người đã hoàn thành bài làm
    public int GetTotalSubmissions()
    => SubmitPapers.Any()
        ? SubmitPapers.Where(x => x.Status == SubmitPaperStatus.End).Count()
        : 0;

    // 
    public int GetTotalStudentsNeedTake()
    {
        // lấy học sinh được asign riêng
        var studentIds = PaperAccesses.Where(x => x.UserId.HasValue).Select(x => x.UserId.Value).ToList();

        //học sinh trong lớp được assign
        var studentIdsInClass = PaperAccesses
            .Where(x => x.ClassId.HasValue && x.Class?.UserClasses != null)
            .SelectMany(x => x.Class.UserClasses!.Select(uc => uc.StudentId)).ToList();

        // Thêm những studentIds mà không có trong studentInclass
        var allStudentIds = studentIdsInClass.Union(studentIds.Except(studentIdsInClass)).ToList();

        return allStudentIds.Count;
    }
}
