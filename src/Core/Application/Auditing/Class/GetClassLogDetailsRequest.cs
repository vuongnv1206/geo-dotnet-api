namespace FSH.WebApi.Application.Auditing.Class;
public class GetClassLogDetailsRequest : IRequest<ClassLogDto>
{
    public Guid ClassId { get; set; }
    public GetClassLogDetailsRequest(Guid classId)
    {
        ClassId = classId;
    }
}

public class GetClasLogDetailsRequestHandler : IRequestHandler<GetClassLogDetailsRequest, ClassLogDto>
{
    private readonly IAuditService _auditService;
    public GetClasLogDetailsRequestHandler(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public async Task<ClassLogDto> Handle(GetClassLogDetailsRequest request, CancellationToken cancellationToken)
    {
        return await _auditService.GetClassLogDetails(request.ClassId);
    }
}
