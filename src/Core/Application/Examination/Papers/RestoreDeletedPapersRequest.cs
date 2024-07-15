namespace FSH.WebApi.Application.Examination.Papers;
public class RestoreDeletedPapersRequest : IRequest<List<Guid>>
{
    public List<Guid> PaperIds { get; set; }
}

public class RestoreDeletedPapersRequestValidator : AbstractValidator<RestoreDeletedPapersRequest>
{
    public RestoreDeletedPapersRequestValidator()
    {
        RuleFor(x => x.PaperIds).NotEmpty();
    }
}


public class RestoreDeletedPapersRequestHandler : IRequestHandler<RestoreDeletedPapersRequest, List<Guid>>
{
    private readonly IPaperService _paperService;
    private readonly ICurrentUser _currentUser;

    public RestoreDeletedPapersRequestHandler(IPaperService paperService, ICurrentUser currentUser)
    {
        _paperService = paperService;
        _currentUser = currentUser;
    }

    public async Task<List<Guid>> Handle(RestoreDeletedPapersRequest request, CancellationToken cancellationToken)
    {
        return await _paperService.RestoreDeletedPapers(_currentUser.GetUserId(), request.PaperIds, cancellationToken);
    }
}