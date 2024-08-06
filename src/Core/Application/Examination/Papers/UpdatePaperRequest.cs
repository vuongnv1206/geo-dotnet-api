using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Examination.PaperLabels;
using FSH.WebApi.Application.Subjects;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Examination.Enums;
using FSH.WebApi.Domain.Subjects;
using Mapster;

namespace FSH.WebApi.Application.Examination.Papers;
public class UpdatePaperRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string ExamName { get; set; }
    public PaperStatus Status { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? Duration { get; set; }
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

    public UpdatePaperRequestHandler(
        IRepositoryWithEvents<Paper> paperRepo,
        IStringLocalizer<UpdatePaperRequestHandler> t,
        IReadRepository<PaperFolder> folderRepo,
        IReadRepository<PaperLabel> labelRepo,
        IReadRepository<Subject> subjectRepo,
        ICurrentUser currentUser)
    {
        _paperRepo = paperRepo;
        _t = t;
        _folderRepo = folderRepo;
        _labelRepo = labelRepo;
        _subjectRepo = subjectRepo;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType> Handle(UpdatePaperRequest request, CancellationToken cancellationToken)
    {
        var paper = await _paperRepo.FirstOrDefaultAsync(new PaperByIdSpec(request.Id));
        _ = paper
            ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.Id]);

        var userId = _currentUser.GetUserId();
        if (!paper.CanUpdate(userId))
            throw new ForbiddenException(_t["You can not Update paper."]);

        if (request.PaperFolderId.HasValue
            && !await _folderRepo.AnyAsync(new PaperFolderByIdSpec(request.PaperFolderId.Value), cancellationToken))
            throw new NotFoundException(_t["Paper folder {0} Not Found.", request.PaperFolderId]);

        if (request.PaperLabelId.HasValue
            && !await _labelRepo.AnyAsync(new PaperLabelByIdSpec(request.PaperLabelId.Value), cancellationToken))
            throw new NotFoundException(_t["Paper Label {0} Not Found.", request.PaperLabelId]);

        if (request.SubjectId.HasValue && !await _subjectRepo.AnyAsync(new SubjectByIdSpec(request.SubjectId.Value), cancellationToken))
            throw new NotFoundException(_t["Subject {0} Not Found.", request.SubjectId]);

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
            paper.UpdatePaperAccesses(request.ShareType,request.PaperAccesses.Adapt<List<PaperAccess>>());
        }

        await _paperRepo.UpdateAsync(paper, cancellationToken);

        return paper.Id;
    }
}
