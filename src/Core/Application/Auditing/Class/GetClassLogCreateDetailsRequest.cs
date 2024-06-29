namespace FSH.WebApi.Application.Auditing.Class;
public class GetClassLogCreateDetailsRequest : IRequest<ClassLogDto>
{
    public Guid Id { get; set; }

    public GetClassLogCreateDetailsRequest(Guid id) => Id = id;
}

public class GetClassLogCreateDetailsRequestHandler : IRequestHandler<GetClassLogCreateDetailsRequest, ClassLogDto>
{
    private readonly IAuditService _auditService;

    public GetClassLogCreateDetailsRequestHandler(IAuditService auditService) => _auditService = auditService;

    public async Task<ClassLogDto> Handle(GetClassLogCreateDetailsRequest request, CancellationToken cancellationToken)
    {
        return await _auditService.GetClassLogCreateDetails(request.Id);
    }
}
