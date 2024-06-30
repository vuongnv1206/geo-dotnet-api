using FSH.WebApi.Application.Auditing.Class;

namespace FSH.WebApi.Application.Auditing;

public interface IAuditService : ITransientService
{
    Task<List<AuditDto>> GetUserTrailsAsync(Guid userId);
    Task<PaginationResponse<AuditTrailsDto>> GetClassTrailsAsync(GetClassLogsRequest request);
    Task<ClassLogDto> GetClassLogCreateDetails(Guid id);
    Task<AuditTrailsUpdateDetailsDto> GetClassUpdateLogDetails(Guid id);
    Task<ClassLogDto> GetClassLogDeleteDetails(Guid classId);
}