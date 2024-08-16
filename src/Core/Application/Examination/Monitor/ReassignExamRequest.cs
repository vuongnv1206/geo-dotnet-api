namespace FSH.WebApi.Application.Examination.Monitor;

public class ReassignExamRequest : IRequest<DefaultIdType>
{
    public DefaultIdType PaperId { get; set; }
    public DefaultIdType UserId { get; set; }
}

public class ReassignExamRequestHandler : IRequestHandler<ReassignExamRequest, DefaultIdType>
{
    public Task<DefaultIdType> Handle(ReassignExamRequest request, CancellationToken cancellationToken)
    {
        // not implemented
        return Task.FromResult(DefaultIdType.NewGuid());
    }
}