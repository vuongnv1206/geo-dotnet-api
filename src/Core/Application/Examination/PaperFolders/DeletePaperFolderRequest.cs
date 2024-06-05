using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class DeletePaperFolderRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public DeletePaperFolderRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class DeletePaperFolderRequestHandler : IRequestHandler<DeletePaperFolderRequest, DefaultIdType>
{
    private readonly IRepository<PaperFolder> _repository;
    private readonly IRepository<Paper> _paperRepository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;
    public DeletePaperFolderRequestHandler(IRepository<PaperFolder> repository, IStringLocalizer<DeletePaperFolderRequestHandler> t, ICurrentUser currentUser, IRepository<Paper> paperRepository)
    {
        _repository = repository;
        _t = t;
        _currentUser = currentUser;
        _paperRepository = paperRepository;
    }

    public async Task<DefaultIdType> Handle(DeletePaperFolderRequest request, CancellationToken cancellationToken)
    {
        var paperFolder = await _repository.FirstOrDefaultAsync(new PaperFolderByIdSpec(request.Id), cancellationToken);
        _ = paperFolder ?? throw new NotFoundException(_t["PaperFolder {0} Not Found.", request.Id]);

        if (!paperFolder.CanDelete(_currentUser.GetUserId()))
        {
            throw new ForbiddenException(_t["You do not have permission to delete this folder."]);
        }

        // Xóa tất cả các Papers và ChildFolders
        await DeleteAllPapersAndChildFolders(paperFolder, cancellationToken);

        await _repository.DeleteAsync(paperFolder, cancellationToken);

        return request.Id;
    }

    private async Task DeleteAllPapersAndChildFolders(PaperFolder folder, CancellationToken cancellationToken)
    {
        // Xóa tất cả các papers
        foreach (var paper in folder.Papers.ToList())
        {
            await _paperRepository.DeleteAsync(paper, cancellationToken);
        }

        // Xóa tất cả các child folders
        foreach (var childFolder in folder.PaperFolderChildrens.ToList())
        {
            await DeleteAllPapersAndChildFolders(childFolder, cancellationToken);
            await _repository.DeleteAsync(childFolder, cancellationToken);
        }
    }

}
