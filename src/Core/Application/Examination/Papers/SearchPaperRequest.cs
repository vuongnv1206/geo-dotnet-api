


using FSH.WebApi.Application.Examination.PaperLabels;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Papers;
public class SearchPaperRequest : PaginationFilter,IRequest<PaginationResponse<PaperDto>>
{
}


public class SearchPaperRequestHandler : IRequestHandler<SearchPaperRequest, PaginationResponse<PaperDto>>
{
    private readonly IReadRepository<Paper> _repository;

    public SearchPaperRequestHandler(IReadRepository<Paper> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<PaperDto>> Handle(SearchPaperRequest request, CancellationToken cancellationToken)
    {
        var spec = new PaperBySearchSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
    }
}
