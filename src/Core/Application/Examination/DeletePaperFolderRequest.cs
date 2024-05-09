

using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination;
public class DeletePaperFolderRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public DeletePaperFolderRequest(Guid id)
    {
        Id = id;
    }
}

public class DeletePaperFolderRequestHandler : IRequestHandler<DeletePaperFolderRequest, Guid>
{
    private readonly IRepository<PaperFolder> _repository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;
    public DeletePaperFolderRequestHandler(IRepository<PaperFolder> repository, IStringLocalizer<DeletePaperFolderRequestHandler> t, ICurrentUser currentUser)
    {
        _repository = repository;
        _t = t;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(DeletePaperFolderRequest request, CancellationToken cancellationToken)
    {
        var paperFolder = await _repository.FirstOrDefaultAsync(new PaperFolderByIdSpec(request.Id), cancellationToken);


        _ = paperFolder ?? throw new NotFoundException(_t["PaperFolder {0} Not Found."]);

        await DeleteChildrenPaperFolders(paperFolder.Id, cancellationToken);

        await _repository.DeleteAsync(paperFolder);

        return request.Id;
    }

    private async Task DeleteChildrenPaperFolders(Guid parentId, CancellationToken cancellationToken)
    {
        var childrenFolders = await _repository.ListAsync(new PaperFolderByParentIdSpec(parentId), cancellationToken);

        foreach (var childFolder in childrenFolders)
        {
            await DeleteChildrenPaperFolders(childFolder.Id, cancellationToken);
            await _repository.DeleteAsync(childFolder); 
        }
    }
}
