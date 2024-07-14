using System;
namespace FSH.WebApi.Application.Examination.Papers;
public class DeletePapersRequest : IRequest<List<Guid>>
{
    public List<Guid> PaperIds { get; set; }
}

public class DeletePapersRequestValidator : AbstractValidator<DeletePapersRequest>
{
    public DeletePapersRequestValidator()
    {
        RuleFor(x => x.PaperIds).NotEmpty();
    }
}

public class DeletePapersRequestHandler : IRequestHandler<DeletePapersRequest, List<Guid>>
{
    private readonly IPaperService _paperService;
    private readonly ICurrentUser _currentUser;

    public DeletePapersRequestHandler(IPaperService paperService, ICurrentUser currentUser)
    {
        _paperService = paperService;
        _currentUser = currentUser;
    }

    public async Task<List<Guid>> Handle(DeletePapersRequest request, CancellationToken cancellationToken)
    {
        return await _paperService.DeletePapers(_currentUser.GetUserId(), request.PaperIds, cancellationToken);
    }
}
