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

    [HttpGet("class/create/{id}")]
    [OpenApiOperation("Get class create log", "")]
    public Task<ClassLogDto> GetClassLogCreateDetails(Guid id)
    {
        return Mediator.Send(new GetClassLogCreateDetailsRequest(id));
    }

    [HttpGet("class/update/{id}")]
    [OpenApiOperation("Get class update log", "")]
    public Task<AuditTrailsUpdateDetailsDto> GetClassUpdateLog(Guid id)
    {
        return Mediator.Send(new GetClassLogUpdateRequest(id));
    }

    [HttpGet("class/delete/{id}")]
    [OpenApiOperation("Get class delete log details", "")]
    public Task<ClassLogDto> GetClassLogDetails(Guid id)
    {
        return Mediator.Send(new GetClassLogDeleteDetailsRequest(id));
    }
}
