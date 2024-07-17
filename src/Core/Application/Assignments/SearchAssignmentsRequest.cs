using FSH.WebApi.Application.Assignments.Dtos;
using FSH.WebApi.Application.Class;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Assignments;
public class SearchAssignmentsRequest : PaginationFilter, IRequest<PaginationResponse<AssignmentDto>>
{
    public Guid? SubjectId { get; set; }
    public decimal? MinimumRate { get; set; }
    public decimal? MaximumRate { get; set; }
    public Guid? ClassId { get; set; }
}

public class SearchAssignmentsRequestHandler : IRequestHandler<SearchAssignmentsRequest, PaginationResponse<AssignmentDto>>
{
    private readonly IReadRepository<Assignment> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<Classes> _classRepo;
    private readonly IStringLocalizer _t;

    public SearchAssignmentsRequestHandler(
        IReadRepository<Assignment> repository,
        ICurrentUser currentUser,
        IRepository<Classes> classRepo,
        IStringLocalizer<SearchAssignmentsRequestHandler> t)
    {
        _repository = repository;
        _currentUser = currentUser;
        _classRepo = classRepo;
        _t = t;
    }

    public async Task<PaginationResponse<AssignmentDto>> Handle(SearchAssignmentsRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();
        if (request.ClassId.HasValue)
        {
            var classroom = await _classRepo.FirstOrDefaultAsync(
                new ClassByIdSpec(request.ClassId.Value, currentUserId), cancellationToken)
                ?? throw new NotFoundException(_t["Classroom Not Found"]);
        }

        var spec = new AssignmentsBySearchRequestWithSubjectsSpec(request, currentUserId);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}