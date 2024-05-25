using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Examination.Papers;
public class GetPaperByIdRequest : IRequest<PaperDto>
{
    public Guid Id { get; set; }
    public GetPaperByIdRequest(Guid id)
    {
        Id = id;
    }
}

public class GetPaperByIdRequestHandler : IRequestHandler<GetPaperByIdRequest, PaperDto>
{
    private readonly IRepository<Paper> _repository;
    private readonly IStringLocalizer _t;

    public GetPaperByIdRequestHandler(IRepository<Paper> repository, IStringLocalizer<GetPaperByIdRequestHandler> t)
    {
        _repository = repository;
        _t = t;
    }

    public async Task<PaperDto> Handle(GetPaperByIdRequest request, CancellationToken cancellationToken)
    {
        var spec = new PaperByIdSpec(request.Id);
        var paper = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
        _ = paper ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.Id]);

        return paper.Adapt<PaperDto>();

    }
}
