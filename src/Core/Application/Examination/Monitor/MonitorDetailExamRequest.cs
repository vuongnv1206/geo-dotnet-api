using FSH.WebApi.Application.Examination.Monitor.Dtos;

namespace FSH.WebApi.Application.Examination.Monitor;
public class MonitorDetailExamRequest : IRequest<MonitorDetailExamDto>
{
    public DefaultIdType PaperId { get; set; }
    public DefaultIdType UserId { get; set; }
}

public class MonitorDetailExamRequestHandler : IRequestHandler<MonitorDetailExamRequest, MonitorDetailExamDto>
{

    Task<MonitorDetailExamDto> IRequestHandler<MonitorDetailExamRequest, MonitorDetailExamDto>.Handle(MonitorDetailExamRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}