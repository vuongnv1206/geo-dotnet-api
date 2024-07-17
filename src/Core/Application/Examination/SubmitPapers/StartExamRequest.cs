using FSH.WebApi.Application.Examination.Papers.Dtos;

namespace FSH.WebApi.Application.Examination.SubmitPapers;

public class StartExamRequest : IRequest<PaperForStudentDto>
{
    public DefaultIdType PaperId { get; set; }

    public required string DeviceId { get; set; }

    public string? DeviceName { get; set; }
    public string? DeviceType { get; set; }
    public string? PublicIp { get; set; }
    public string? LocalIp { get; set; }
    public bool IsResume { get; set; }
}

public class StartExamRequestValidator : CustomValidator<StartExamRequest>
{
    public StartExamRequestValidator()
    {
        _ = RuleFor(x => x.PaperId)
            .NotEmpty();
        _ = RuleFor(x => x.DeviceId)
            .NotEmpty();
    }
}

public class StartExamRequestHandler : IRequestHandler<StartExamRequest, PaperForStudentDto>
{
    private readonly ISubmmitPaperService _submmitPaperService;

    public StartExamRequestHandler(ISubmmitPaperService submmitPaperService)
    {
        _submmitPaperService = submmitPaperService;
    }

    public Task<PaperForStudentDto> Handle(StartExamRequest request, CancellationToken cancellationToken)
    {
        return _submmitPaperService.StartExamAsync(request, cancellationToken);
    }
}