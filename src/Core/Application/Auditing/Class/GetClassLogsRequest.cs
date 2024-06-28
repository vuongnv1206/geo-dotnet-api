namespace FSH.WebApi.Application.Auditing.Class;
public class GetClassLogsRequest : IRequest<PaginationResponse<AuditTrailsDto>>
{
    public Guid UserId { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? Action { get; set; }
}

public class GetClassLogsRequestHandler : IRequestHandler<GetClassLogsRequest, PaginationResponse<AuditTrailsDto>>
{
    private readonly IAuditService _auditService;
    private readonly ICurrentUser _currentUser;
    public GetClassLogsRequestHandler(IAuditService auditService, ICurrentUser currentUser)
    {
        _auditService = auditService;
        _currentUser = currentUser;
    }

    public async Task<PaginationResponse<AuditTrailsDto>> Handle(GetClassLogsRequest request, CancellationToken cancellationToken)
    {
        request.UserId = _currentUser.GetUserId();
        return await _auditService.GetClassTrailsAsync(request);
    }
}