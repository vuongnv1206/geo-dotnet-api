using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperLabels;
public class GetPaperLabelRequest : IRequest<PaperLabelDto>
{
    public Guid Id { get; set; }
    public GetPaperLabelRequest(Guid id)
    {
        Id = id;
    }
}

public class GetPaperLabelRequestHandler : IRequestHandler<GetPaperLabelRequest, PaperLabelDto>
{
    private readonly IRepository<PaperLabel> _repository;
    private readonly IStringLocalizer _t;

    public GetPaperLabelRequestHandler(IRepository<PaperLabel> repository, IStringLocalizer<GetPaperLabelRequestHandler> localizer) => (_repository, _t) = (repository, localizer);

    public async Task<PaperLabelDto> Handle(GetPaperLabelRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            (ISpecification<PaperLabel, PaperLabelDto>)new PaperLabelByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(_t["PaperLabel {0} Not Found.", request.Id]);
}