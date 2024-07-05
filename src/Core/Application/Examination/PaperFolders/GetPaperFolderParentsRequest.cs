using FSH.WebApi.Domain.Examination;
using Mapster;


namespace FSH.WebApi.Application.Examination.PaperFolders;
public class GetPaperFolderParentsRequest : IRequest<List<PaperFolderDto>>
{
    public Guid PaperFolderId { get; set; }

    public GetPaperFolderParentsRequest(Guid paperFolderId)
    {
        PaperFolderId = paperFolderId;
    }
}

public class GetPaperFolderParentsRequestHandler : IRequestHandler<GetPaperFolderParentsRequest, List<PaperFolderDto>>
{
    private readonly IReadRepository<PaperFolder> _paperFolderRepo;
    public readonly ICurrentUser _currentUser;
    public GetPaperFolderParentsRequestHandler(IReadRepository<PaperFolder> paperFolderRepo, ICurrentUser currentUser)
    {
        _paperFolderRepo = paperFolderRepo;
        _currentUser = currentUser;
    }

    public async Task<List<PaperFolderDto>> Handle(GetPaperFolderParentsRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();

        var paperFolders = await _paperFolderRepo.ListAsync(new MyPaperFolderTreeByIdSpec(currentUserId), cancellationToken);
        var paperFolder = paperFolders.FirstOrDefault(x => x.Id == request.PaperFolderId);
        if (paperFolder == null)
        {
            throw new NotFoundException($"PaperFolder with Id {request.PaperFolderId} not found.");
        }

        var parents = paperFolder.ListParents();

        return parents.Adapt<List<PaperFolderDto>>();
    }
}
