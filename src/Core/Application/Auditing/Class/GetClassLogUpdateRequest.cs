namespace FSH.WebApi.Application.Auditing.Class;
public class GetClassLogUpdateRequest : IRequest<AuditTrailsUpdateDetailsDto>
{
    public Guid Id { get; set; }
    public GetClassLogUpdateRequest(Guid id)
    {
        Id = id;
    }
}

public class GetClassUpdateLogDtoHandler : IRequestHandler<GetClassLogUpdateRequest, AuditTrailsUpdateDetailsDto>
{
    private readonly IAuditService _auditService;

    public GetClassUpdateLogDtoHandler(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public async Task<AuditTrailsUpdateDetailsDto> Handle(GetClassLogUpdateRequest request, CancellationToken cancellationToken)
    {
        return await _auditService.GetClassUpdateLogDetails(request.Id);
    }
}
