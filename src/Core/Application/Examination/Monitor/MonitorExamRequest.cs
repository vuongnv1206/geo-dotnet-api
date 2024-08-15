using FSH.WebApi.Application.Examination.Monitor.Dtos;

namespace FSH.WebApi.Application.Examination.Monitor;

public class MonitorExamRequest : IRequest<MonitorExamDto>
{
    public DefaultIdType PaperId { get; set; }
}

public class MonitorExamRequestHandler : IRequestHandler<MonitorExamRequest, MonitorExamDto>
{
    private readonly IMonitorService _monitorService;

    public MonitorExamRequestHandler(IMonitorService monitorService)
    {
        _monitorService = monitorService;
    }

    public Task<MonitorExamDto> Handle(MonitorExamRequest request, CancellationToken cancellationToken)
    {
        return _monitorService.MonitorExam(request.PaperId);
    }
}