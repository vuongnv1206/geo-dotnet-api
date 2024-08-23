using FSH.WebApi.Application.Examination.Monitor.Dtos;

namespace FSH.WebApi.Application.Examination.Monitor;
public interface IMonitorService : ITransientService
{
    Task<MonitorExamDto> MonitorExam(DefaultIdType paperId);
}
