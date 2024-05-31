using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperLabels;
public class SearchPaperLabelRequest : PaginationFilter, IRequest<PaginationResponse<PaperLabelDto>>
{
}

public class SearchPaperLabelRequestHandler : IRequestHandler<SearchPaperLabelRequest, PaginationResponse<PaperLabelDto>>
{
    private readonly IReadRepository<PaperLabel> _repository;

    public SearchPaperLabelRequestHandler(IReadRepository<PaperLabel> repository) => _repository = repository;

    public async Task<PaginationResponse<PaperLabelDto>> Handle(SearchPaperLabelRequest request, CancellationToken cancellationToken)
    {
        var spec = new PaperLabelsBySearchRequestSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
    }
}