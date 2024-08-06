namespace FSH.WebApi.Application.Examination.SubmitPapers;

public class SubmitExamRequest : IRequest<Guid>
{
    public required string SubmitPaperData { get; set; }
    public string? PublicIp { get; set; }
    public bool IsEnd { get; set; }
}

public class SubmitExamRequestValidator : CustomValidator<SubmitExamRequest>
{
    public SubmitExamRequestValidator()
    {
        _ = RuleFor(x => x.SubmitPaperData)
            .NotEmpty();
    }
}

public class SubmitExamRequestHandler : IRequestHandler<SubmitExamRequest, Guid>
{
    private readonly ISubmmitPaperService _submmitPaperService;

    public SubmitExamRequestHandler(ISubmmitPaperService submmitPaperService)
    {
        _submmitPaperService = submmitPaperService;
    }

    public Task<Guid> Handle(SubmitExamRequest request, CancellationToken cancellationToken)
    {
        return _submmitPaperService.SubmitExamAsync(request, cancellationToken);
    }
}