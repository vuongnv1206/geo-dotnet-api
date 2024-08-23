using FSH.WebApi.Application.Assignments.Dtos;
using FSH.WebApi.Application.Class;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Shared.Authorization;

namespace FSH.WebApi.Application.Assignments;
public class SearchMyAssignmentsRequest : PaginationFilter, IRequest<PaginationResponse<AssignmentDto>>
{
    public Guid? SubjectId { get; set; }
    public Guid? ClassId { get; set; }
}

public class SearchMyAssignmentsRequestHandler : IRequestHandler<SearchMyAssignmentsRequest, PaginationResponse<AssignmentDto>>
{
    private readonly IReadRepository<Assignment> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<Classes> _classRepo;
    private readonly IStringLocalizer _t;

    public SearchMyAssignmentsRequestHandler(
        IReadRepository<Assignment> repository,
        ICurrentUser currentUser,
        IRepository<Classes> classRepo,
        IStringLocalizer<SearchMyAssignmentsRequestHandler> t)
    {
        _repository = repository;
        _currentUser = currentUser;
        _classRepo = classRepo;
        _t = t;
    }

    public async Task<PaginationResponse<AssignmentDto>> Handle(SearchMyAssignmentsRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();
        if (request.ClassId.HasValue)
        {
            var classroom = await _classRepo.FirstOrDefaultAsync(
                new ClassByIdSpec(request.ClassId.Value, currentUserId), cancellationToken)
                ?? throw new NotFoundException(_t["Classroom Not Found"]);
        }
        
        var spec = new AssignmentsBySearchSpec(request, currentUserId ,_currentUser.IsInRole(FSHRoles.Student));

        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}