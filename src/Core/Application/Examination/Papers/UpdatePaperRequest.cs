using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Examination.PaperLabels;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Examination.Enums;

namespace FSH.WebApi.Application.Examination.Papers;
public class UpdatePaperRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public required string ExamName { get; set; }
    public PaperStatus Status { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public Guid? PaperLabelId { get; set; }
    public int? Duration { get; set; }
    public bool Shuffle { get; set; }
    public bool ShowMarkResult { get; set; }
    public bool ShowQUestionAnswer { get; set; }
    public string? Password { get; set; }
    public PaperType Type { get; set; }
    public Guid? FolderId { get; set; }
    public bool IsPublish { get; set; }
    public string? Content { get; set; }
    public string? Description { get; set; }

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
    private readonly ICurrentUser _currentUser;

    public UpdatePaperRequestHandler(
        IRepositoryWithEvents<Paper> paperRepo,
        IStringLocalizer<UpdatePaperRequestHandler> t,
        IReadRepository<PaperFolder> folderRepo,
        IReadRepository<PaperLabel> labelRepo,
        ICurrentUser currentUser)
    {
        _paperRepo = paperRepo;
        _t = t;
        _folderRepo = folderRepo;
        _labelRepo = labelRepo;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType> Handle(UpdatePaperRequest request, CancellationToken cancellationToken)
    {
        var paper = await _paperRepo.GetByIdAsync(request.Id);
        _ = paper
            ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.Id]);

        var userId = _currentUser.GetUserId();
        if (!paper.CanUpdate(userId))
            throw new ForbiddenException(_t["You can not Update paper."]);

        if (request.FolderId.HasValue
            && !await _folderRepo.AnyAsync(new PaperFolderByIdSpec(request.FolderId.Value), cancellationToken))
            throw new NotFoundException(_t["Paper folder {0} Not Found.", request.FolderId]);

        if (request.PaperLabelId.HasValue
            && !await _labelRepo.AnyAsync(new PaperLabelByIdSpec(request.PaperLabelId.Value), cancellationToken))
            throw new NotFoundException(_t["Paper Label {0} Not Found.", request.PaperLabelId]);

        paper.Update(
            request.ExamName,
            request.Status,
            request.StartTime,
            request.EndTime,
            request.PaperLabelId,
            request.Duration,
            request.Shuffle,
            request.ShowMarkResult,
            request.ShowQUestionAnswer,
            request.Type,
            request.FolderId,
            request.IsPublish,
            request.Content,
            request.Description,
            request.Password);

        await _paperRepo.UpdateAsync(paper, cancellationToken);

        return paper.Id;
    }
}
