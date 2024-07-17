using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class;
public class SearchClassesRequest : PaginationFilter, IRequest<PaginationResponse<ClassDto>>
{
    public Guid? GroupClassId { get; set; }
}

public class SearchClassesRequestHandler : IRequestHandler<SearchClassesRequest, PaginationResponse<ClassDto>>
{
    private readonly IReadRepository<Classes> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;
    public SearchClassesRequestHandler(
        IReadRepository<Classes> repository,
        ICurrentUser currentUser,
        IStringLocalizer<SearchClassesRequestHandler> localizer)
    {
        _repository = repository;
        _currentUser = currentUser;
        _t = localizer;
    }

    public async Task<PaginationResponse<ClassDto>> Handle(SearchClassesRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var spec = new ClassesBySearchRequestWithGroupClassSpec(request, userId);
        var data = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
        return data;
    }
}
