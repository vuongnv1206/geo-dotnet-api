using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class CreateSubmitPaperRequest : IRequest<Guid>
{
    public Guid PaperId { get; set; }
    public SubmitPaperStatus Status { get; set; }
}

public class CreateSubmitPaperRequestValidator : CustomValidator<CreateSubmitPaperRequest>
{
    public CreateSubmitPaperRequestValidator(
        IRepository<Paper> paperRepository,
        IStringLocalizer<CreateSubmitPaperRequestValidator> T)
    {
        RuleFor(x => x.PaperId)
            .MustAsync(async (id, ct) => await paperRepository.FirstOrDefaultAsync(new PaperByIdSpec(id), ct) is not null)
                .WithMessage((_, paperId) => T["Paper {0} Not Found.", paperId]);
    }
}

public class CreateSubmitPaperRequestHandler : IRequestHandler<CreateSubmitPaperRequest, Guid>
{
    private readonly IRepositoryWithEvents<SubmitPaper> _repo;

    public CreateSubmitPaperRequestHandler(
        IRepositoryWithEvents<SubmitPaper> repo)
    {
        _repo = repo;
    }

    public async Task<DefaultIdType> Handle(CreateSubmitPaperRequest request, CancellationToken cancellationToken)
    {
        var submitPaper = new SubmitPaper(request.PaperId, request.Status);
        await _repo.AddAsync(submitPaper, cancellationToken);

        return submitPaper.Id;
    }
}
