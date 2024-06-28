namespace FSH.WebApi.Application.Auditing.Class;
public class GetClassUpdateLogRequest : IRequest<AuditTrailsDetailsDto<ClassLogDto>>
{
    public Guid Id { get; set; }
    public GetClassUpdateLogRequest(Guid id)
    {
        Id = id;
    }
}

public class GetClassUpdateLogDtoHandler : IRequestHandler<GetClassUpdateLogRequest, AuditTrailsDetailsDto<ClassLogDto>>
{
    private readonly IAuditService _auditService;

    public GetClassUpdateLogDtoHandler(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public async Task<AuditTrailsDetailsDto<ClassLogDto>> Handle(GetClassUpdateLogRequest request, CancellationToken cancellationToken)
    {
        return await _auditService.GetClassUpdateLogDetails(request.Id);
    }
}
