using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Examination.PaperLabels;
using FSH.WebApi.Application.Notifications;
using FSH.WebApi.Application.Subjects;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Examination.Enums;
using FSH.WebApi.Domain.Subjects;
using Mapster;
using MediatR;

namespace FSH.WebApi.Application.Examination.Papers;
public class UpdatePaperRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string ExamName { get; set; }
    public PaperStatus Status { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public float? Duration { get; set; }
    public bool Shuffle { get; set; }
    public ShowResult ShowMarkResult { get; set; }
    public ShowQuestionAnswer ShowQuestionAnswer { get; set; }
    public string? Password { get; set; }
    public int? NumberAttempt { get; set; }
    public PaperShareType ShareType { get; set; }
    public PaperType Type { get; set; }
    public bool IsPublish { get; set; }
    public string? Content { get; set; }
    public string? Description { get; set; }
    public Guid? PaperLabelId { get; set; }
    public Guid? SubjectId { get; set; }
    public Guid? PaperFolderId { get; set; }
    public string? PublicIpAllowed { get; set; }
    public string? LocalIpAllowed { get; set; }
    public List<PaperAccessDto>? PaperAccesses { get; set; }

}

public class UpdatePaperRequestValidator : CustomValidator<UpdatePaperRequest>
{
    public UpdatePaperRequestValidator(IStringLocalizer<UpdatePaperRequestValidator> T)
    {
        RuleFor(x => x.ExamName)
            .NotEmpty();

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
                .WithMessage(T["End time must to greater than Start time"]);
    }
}

public class UpdatePaperRequestHandler : IRequestHandler<UpdatePaperRequest, Guid>
{
    private readonly IRepositoryWithEvents<Paper> _paperRepo;
    private readonly IStringLocalizer<UpdatePaperRequestHandler> _t;
    private readonly IReadRepository<PaperFolder> _folderRepo;
    private readonly IReadRepository<PaperLabel> _labelRepo;
    private readonly IReadRepository<Subject> _subjectRepo;
    private readonly ICurrentUser _currentUser;
    private readonly INotificationService _notificationService;
    private readonly IRepository<Classes> _repoClass;

    public UpdatePaperRequestHandler(
        IRepositoryWithEvents<Paper> paperRepo,
        IStringLocalizer<UpdatePaperRequestHandler> t,
        IReadRepository<PaperFolder> folderRepo,
        IReadRepository<PaperLabel> labelRepo,
        IReadRepository<Subject> subjectRepo,
        ICurrentUser currentUser,
        INotificationService notificationService,
        IRepository<Classes> repositoryClass)
    {
        _paperRepo = paperRepo;
        _t = t;
        _folderRepo = folderRepo;
        _labelRepo = labelRepo;
        _subjectRepo = subjectRepo;
        _currentUser = currentUser;
        _notificationService = notificationService;
        _repoClass = repositoryClass;
    }

    public async Task<DefaultIdType> Handle(UpdatePaperRequest request, CancellationToken cancellationToken)
    {
        var paper = await _paperRepo.FirstOrDefaultAsync(new PaperByIdSpec(request.Id));
        _ = paper
            ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.Id]);


        var userId = _currentUser.GetUserId();
        if (!paper.CanUpdate(userId))
            throw new ForbiddenException(_t["You do not have permission to edit this paper."]);


        if (request.PaperFolderId.HasValue
            && !await _folderRepo.AnyAsync(new PaperFolderByIdSpec(request.PaperFolderId.Value), cancellationToken))
            throw new NotFoundException(_t["Paper folder {0} Not Found.", request.PaperFolderId]);

        if (request.PaperLabelId.HasValue
            && !await _labelRepo.AnyAsync(new PaperLabelByIdSpec(request.PaperLabelId.Value), cancellationToken))
            throw new NotFoundException(_t["Paper Label {0} Not Found.", request.PaperLabelId]);

        if (request.SubjectId.HasValue && !await _subjectRepo.AnyAsync(new SubjectByIdSpec(request.SubjectId.Value), cancellationToken))
            throw new NotFoundException(_t["Subject {0} Not Found.", request.SubjectId]);

        //Nếu muốn publish
        if (request.Status == PaperStatus.Publish)
        {
            // Kiểm tra nếu Paper đã được Publish
            if (paper.Status == PaperStatus.Publish)
            {
                var timeNow = DateTime.UtcNow;
                // Kiểm tra nếu đang trong giai đoạn 30 phút trước khi thi, đang thi hoặc đã thi xong
                if (paper.StartTime.HasValue)
                {
                    var startTime = paper.StartTime.Value;

                    // Nếu đã vượt quá EndTime, không cho phép cập nhật
                    if (paper.EndTime.HasValue && timeNow > paper.EndTime.Value)
                    {
                        throw new ConflictException(_t["Cannot update setting after the exam has ended."]);
                    }

                    // Nếu đang trong khoảng 30 phút trước StartTime hoặc trong thời gian thi, không cho phép cập nhật
                    if (timeNow > startTime.AddMinutes(-30) && timeNow < paper.EndTime)
                    {
                        throw new ConflictException(_t["Cannot update the paper within 30 minutes before the start time or during the exam."]);
                    }
                }


                //Chỉ cho phép sửa Time và send notification
                if (request.StartTime.HasValue && request.EndTime.HasValue)
                {
                    bool isStartTimeChanged = paper.StartTime != request.StartTime.Value;
                    bool isEndTimeChanged = paper.EndTime != request.EndTime.Value;

                    // Gửi thông báo khi thời gian thi được cập nhật
                    if (isStartTimeChanged || isEndTimeChanged) //nếu changes
                    {

                        // Cập nhật StartTime và EndTime
                        paper.UpdateTime(request.StartTime, request.EndTime, request.Duration);
                        await _paperRepo.UpdateAsync(paper, cancellationToken);
                        if (request.PaperAccesses is not null && request.PaperAccesses.Any())
                        {
                            var studentIds = await GetStudentIdsAsync(request.PaperAccesses, cancellationToken);
                            if (studentIds.Count > 0)
                            {
                                var notification = new BasicNotification
                                {
                                    Title = "Exam Schedule Updated",
                                    Message = $"The exam '{paper.ExamName}' has had its schedule updated. Please check the new schedule",
                                    Label = BasicNotification.LabelType.Information,
                                };

                                await _notificationService.SendNotificationToUsers(studentIds, notification, null, cancellationToken);
                            }
                        }
                    }
                }
                else
                {
                    throw new ConflictException(_t["When publishing, StartTime and EndTime are required."]);
                }
            }
            else  //  Paper đã đc Save as draft
            {
                if (!request.StartTime.HasValue && !request.EndTime.HasValue)
                {
                    throw new ConflictException(_t["When publishing, StartTime and EndTime are required."]);
                }
                //Gửi noti đến những ai thi
                if (request.PaperAccesses is not null && request.PaperAccesses.Any())
                {

                    paper.UpdatePaperAccesses(request.ShareType, request.PaperAccesses.Adapt<List<PaperAccess>>());
                    paper.Update(
                   request.ExamName,
                   request.Status,
                   request.StartTime,
                   request.EndTime,
                   request.Duration,
                   request.Shuffle,
                   request.ShowMarkResult,
                   request.ShowQuestionAnswer,
                   request.Type,
                   request.IsPublish,
                   request.Content,
                   request.Description,
                   request.Password,
                   request.NumberAttempt,
                   request.ShareType,
                   request.PaperFolderId,
                   request.PaperLabelId,
                   request.SubjectId,
                   request.PublicIpAllowed,
                   request.LocalIpAllowed
                   );
                    await _paperRepo.UpdateAsync(paper, cancellationToken);
                    var studentIds = await GetStudentIdsAsync(request.PaperAccesses, cancellationToken);
                    if (studentIds.Count > 0)
                    {
                        var notification = new BasicNotification
                        {
                            Title = "Exam Schedule Updated",
                            Message = $"The exam '{paper.ExamName}'  has been published. Please check the schedule and prepare yourself",
                            Label = BasicNotification.LabelType.Information,
                        };

                        await _notificationService.SendNotificationToUsers(studentIds, notification, null, cancellationToken);
                    }
                }
                else
                {
                    throw new ConflictException(_t["Cannot publish the exam because there are no students or classes assigned to it. Please assign at least one student or class before publishing."]);
                }
            }
        }
        else  //Save as draft
        {
            // Kiểm tra nếu Paper đã được Publish
            if (paper.Status == PaperStatus.Publish)
            {
                throw new ConflictException(_t["Cannot update setting of exam as it has already been published."]);
            }
            else
            {
                paper.Update(
              request.ExamName,
              request.Status,
              request.StartTime,
              request.EndTime,
              request.Duration,
              request.Shuffle,
              request.ShowMarkResult,
              request.ShowQuestionAnswer,
              request.Type,
              request.IsPublish,
              request.Content,
              request.Description,
              request.Password,
              request.NumberAttempt,
              request.ShareType,
              request.PaperFolderId,
              request.PaperLabelId,
              request.SubjectId,
              request.PublicIpAllowed,
              request.LocalIpAllowed
              );

                if (request.PaperAccesses is not null)
                {
                    paper.UpdatePaperAccesses(request.ShareType, request.PaperAccesses.Adapt<List<PaperAccess>>());
                }

                await _paperRepo.UpdateAsync(paper, cancellationToken);
            }

        }



        return paper.Id;
    }
    private async Task<List<string>> GetStudentIdsAsync(List<PaperAccessDto> paperAccess, CancellationToken cancellationToken)
    {
        var studentIds = new List<string>();

        foreach (var access in paperAccess)
        {
            if (access.ClassId.HasValue)
            {
                var classroom = await _repoClass.FirstOrDefaultAsync(new ClassesByIdSpec(access.ClassId.Value), cancellationToken);
                if (classroom != null)
                {
                    studentIds.AddRange(classroom.GetStudentIds().Where(id => id.HasValue).Select(id => id.Value.ToString()));
                }
            }

            if (access.UserId.HasValue)
            {
                studentIds.Add(access.UserId.Value.ToString());
            }
        }

        return studentIds.Distinct().ToList();
    }
}
