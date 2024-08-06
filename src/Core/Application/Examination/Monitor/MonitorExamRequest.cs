namespace FSH.WebApi.Application.Examination.Monitor;

public class MonitorExamRequest : IRequest<DefaultIdType>
{
    public DefaultIdType PaperId { get; set; }
}

public class MonitorExamRequestHandler : IRequestHandler<MonitorExamRequest, DefaultIdType>
{
    public Task<DefaultIdType> Handle(MonitorExamRequest request, CancellationToken cancellationToken)
    {
        // not implemented
        return Task.FromResult(new DefaultIdType());
    }
}