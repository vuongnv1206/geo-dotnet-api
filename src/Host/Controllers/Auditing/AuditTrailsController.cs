using FSH.WebApi.Application.Auditing;
using FSH.WebApi.Application.Auditing.Class;

namespace FSH.WebApi.Host.Controllers.Auditing;
public class AuditTrailsController : VersionedApiController
{

    [HttpPost("class")]
    [OpenApiOperation("Get class logs", "")]
    public Task<PaginationResponse<AuditTrailsDto>> GetClassLogs(GetClassLogsRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("class/update/{id}")]
    [OpenApiOperation("Get class update log", "")]
    public Task<AuditTrailsDetailsDto<ClassLogDto>> GetClassUpdateLog(Guid id)
    {
        return Mediator.Send(new GetClassUpdateLogRequest(id));
    }

    [HttpGet("class/details/{id}")]
    [OpenApiOperation("Get class log details", "")]
    public Task<ClassLogDto> GetClassLogDetails(Guid id)
    {
        return Mediator.Send(new GetClassLogDetailsRequest(id));
    }
}
