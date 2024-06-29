namespace FSH.WebApi.Application.Auditing.Class;
public class GetClassLogDeleteDetailsRequest : IRequest<ClassLogDto>
{
    public Guid ClassId { get; set; }
    public GetClassLogDeleteDetailsRequest(Guid classId)
    {
        ClassId = classId;
    }
}

public class GetClasLogDetailsRequestHandler : IRequestHandler<GetClassLogDeleteDetailsRequest, ClassLogDto>
{
    private readonly IAuditService _auditService;
    public GetClasLogDetailsRequestHandler(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public async Task<ClassLogDto> Handle(GetClassLogDeleteDetailsRequest request, CancellationToken cancellationToken)
    {
        return await _auditService.GetClassLogDeleteDetails(request.ClassId);
    }
}
