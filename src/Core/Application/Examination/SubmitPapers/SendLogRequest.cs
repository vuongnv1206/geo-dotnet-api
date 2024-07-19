namespace FSH.WebApi.Application.Examination.SubmitPapers;

public class SendLogRequest : IRequest<DefaultIdType>
{
    public DefaultIdType SubmitPaperId { get; set; }
    public string? DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public string? DeviceType { get; set; }
    public string? PublicIp { get; set; }
    public string? LocalIp { get; set; }
    public string? ProcessLog { get; set; }
    public string? MouseLog { get; set; }
    public string? KeyboardLog { get; set; }
    public string? NetworkLog { get; set; }
    public bool? IsSuspicious { get; set; }
}

public class SendLogRequestHandler : IRequestHandler<SendLogRequest, DefaultIdType>
{
    private readonly ISubmmitPaperService _submmitPaperService;

    public SendLogRequestHandler(ISubmmitPaperService submmitPaperService)
    {
        _submmitPaperService = submmitPaperService;
    }

    public Task<DefaultIdType> Handle(SendLogRequest request, CancellationToken cancellationToken)
    {
        return _submmitPaperService.SendLogAsync(request, cancellationToken);
    }
}