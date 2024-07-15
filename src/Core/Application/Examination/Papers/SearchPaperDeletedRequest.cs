using FSH.WebApi.Application.Examination.Papers.Dtos;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Papers;
public class SearchPaperDeletedRequest : PaginationFilter, IRequest<PaginationResponse<PaperDeletedDto>>
{
    public string? ExamName { get; set; }
}

public class SearchPaperDeletedRequestSpec : EntitiesByPaginationFilterSpec<Paper, PaperDeletedDto>
{
    public SearchPaperDeletedRequestSpec(SearchPaperDeletedRequest request, Guid userId) : base(request) =>
        Query
        .IgnoreQueryFilters()
        .Include(p => p.Subject)
        .OrderByDescending(p => p.DeletedOn)
        .Where(p => p.DeletedBy.Equals(userId))
        .Where(p => p.ExamName.ToLower().Contains(request.ExamName!.ToLower()), !string.IsNullOrEmpty(request.ExamName));
}

public class SearchPaperDeletedRequestHandler : IRequestHandler<SearchPaperDeletedRequest, PaginationResponse<PaperDeletedDto>>
{
    private readonly IReadRepository<Paper> _repository;
    private readonly ICurrentUser _currentUser;

    public SearchPaperDeletedRequestHandler(IReadRepository<Paper> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PaginationResponse<PaperDeletedDto>> Handle(SearchPaperDeletedRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var spec = new SearchPaperDeletedRequestSpec(request, userId);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}
