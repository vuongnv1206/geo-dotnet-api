using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class SearchPaperFolderRequest : IRequest<List<PaperFolderDto>>
{
    public Guid? ParentId { get; set; }
    public string? Name { get; set; }
}

public class SearchPaperFolderRequestHandler : IRequestHandler<SearchPaperFolderRequest, List<PaperFolderDto>>
{
    public readonly IReadRepository<PaperFolder> _paperFolderRepo;
    public readonly ICurrentUser _currentUser;

    public SearchPaperFolderRequestHandler(
        ICurrentUser currentUser,
        IReadRepository<PaperFolder> paperFolderRepo)
    {
        _currentUser = currentUser;
        _paperFolderRepo = paperFolderRepo;
    }

    public async Task<List<PaperFolderDto>> Handle(SearchPaperFolderRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();
        var spec = new PaperFolderBySearchRequestSpec(request, currentUserId);

        var data = await _paperFolderRepo.ListAsync(spec, cancellationToken);
        var dtos = data.Adapt<List<PaperFolderDto>>();
        return dtos;
    }
}