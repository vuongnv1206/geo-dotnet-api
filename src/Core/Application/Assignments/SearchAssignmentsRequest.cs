using FSH.WebApi.Application.Assignments.Dtos;
using FSH.WebApi.Domain.Assignment;

namespace FSH.WebApi.Application.Assignments;
public class SearchAssignmentsRequest : PaginationFilter, IRequest<PaginationResponse<AssignmentDto>>
{
    public Guid? SubjectId { get; set; }
    public decimal? MinimumRate { get; set; }
    public decimal? MaximumRate { get; set; }
}

public class SearchAssignmentsRequestHandler : IRequestHandler<SearchAssignmentsRequest, PaginationResponse<AssignmentDto>>
{
    private readonly IReadRepository<Assignment> _repository;
    private readonly ICurrentUser _currentUser;


    public SearchAssignmentsRequestHandler(IReadRepository<Assignment> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PaginationResponse<AssignmentDto>> Handle(SearchAssignmentsRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();
        var spec = new AssignmentsBySearchRequestWithSubjectsSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}