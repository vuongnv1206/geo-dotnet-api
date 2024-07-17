using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.SubmitPapers.Specs;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class CreateSubmitPaperRequest : IRequest<Guid>
{
    public Guid PaperId { get; set; }
    public string? Password { get; set; }
}

public class CreateSubmitPaperRequestValidator : CustomValidator<CreateSubmitPaperRequest>
{
    public CreateSubmitPaperRequestValidator(
        IRepository<Paper> paperRepository,
        IStringLocalizer<CreateSubmitPaperRequestValidator> T)
    {
        _ = RuleFor(x => x.PaperId)
            .MustAsync(async (id, ct) => await paperRepository.FirstOrDefaultAsync(new PaperByIdSpec(id), ct) is not null)
                .WithMessage((_, paperId) => T["Paper {0} Not Found.", paperId]);
    }
}

public class CreateSubmitPaperRequestHandler : IRequestHandler<CreateSubmitPaperRequest, Guid>
{
    private readonly IRepositoryWithEvents<SubmitPaper> _repo;
    private readonly IRepository<Paper> _paperRepo;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;

    public CreateSubmitPaperRequestHandler(
        IRepositoryWithEvents<SubmitPaper> repo,
        IRepository<Paper> paperRepo,
        IStringLocalizer<CreateSubmitPaperRequestHandler> t,
        ICurrentUser currentUser)
    {
        _repo = repo;
        _paperRepo = paperRepo;
        _t = t;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType> Handle(CreateSubmitPaperRequest request, CancellationToken cancellationToken)
    {
        var paper = await _paperRepo.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId))
            ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.PaperId]);

        if (!string.IsNullOrEmpty(paper.Password) && paper.Password != request.Password)
        {
            throw new ConflictException(_t["Password wrong"]);
        }

        var userId = _currentUser.GetUserId();

        var submitPapers = await _repo.ListAsync(new SubmitPaperByPaperId(paper, userId));

        if (submitPapers.Count >= paper.NumberAttempt)
        {
            throw new ConflictException(_t["Have used up all your attempts"]);
        }

        var timeNow = DateTime.Now;
        if ((paper.StartTime.HasValue && paper.StartTime < timeNow)
            || (paper.EndTime.HasValue && paper.EndTime < timeNow))
        {
            throw new ConflictException(_t["Over time to do this exam"]);
        }

        var submitPaper = new SubmitPaper(request.PaperId);
        _ = await _repo.AddAsync(submitPaper, cancellationToken);

        return submitPaper.Id;
    }
}
