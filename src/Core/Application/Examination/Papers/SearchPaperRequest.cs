
using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Examination.Papers;
public class SearchPaperRequest : IRequest<List<PaperInListDto>>
{
    public Guid? PaperFolderId { get; set; }
    public string? Name { get; set; }
}


public class SearchPaperRequestHandler : IRequestHandler<SearchPaperRequest, List<PaperInListDto>>
{
    private readonly IReadRepository<Paper> _repository;

    public SearchPaperRequestHandler(IReadRepository<Paper> repository)
    {
        _repository = repository;
    }

    public async Task<List<PaperInListDto>> Handle(SearchPaperRequest request, CancellationToken cancellationToken)
    {
        var spec = new PaperBySearchSpec(request);
       var data = await _repository.ListAsync(spec,cancellationToken);
        return data.Adapt<List<PaperInListDto>>();
    }
}
