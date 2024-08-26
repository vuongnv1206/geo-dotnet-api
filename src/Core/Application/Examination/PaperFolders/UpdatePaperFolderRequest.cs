using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class UpdatePaperFolderRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid? ParentId { get; set; }
}

public class UpdatePaperFolderRequestHandler : IRequestHandler<UpdatePaperFolderRequest, Guid>
{
    private readonly IRepositoryWithEvents<PaperFolder> _paperFolderRepo;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;

    public UpdatePaperFolderRequestHandler(
        IRepositoryWithEvents<PaperFolder> paperFolderRepo,
        ICurrentUser currentUser,
        IStringLocalizer<UpdatePaperFolderRequestHandler> t)
    {
        _paperFolderRepo = paperFolderRepo;
        _currentUser = currentUser;
        _t = t;
    }

    public async Task<DefaultIdType> Handle(UpdatePaperFolderRequest request, CancellationToken cancellationToken)
    {
        var folder = await _paperFolderRepo.FirstOrDefaultAsync(new MyPaperFolderTreeByIdSpec(request.Id), cancellationToken);
        _ = folder ?? throw new NotFoundException(_t["Paper Folder {0} Not Found.", request.Id]);

        var userId = _currentUser.GetUserId();
        if (!folder.CanUpdate(userId))
        {
            throw new ForbiddenException(_t["You do not have permission to edit this folder."]);
        }
        var treeFolder = await _paperFolderRepo.ListAsync(new PaperFolderTreeSpec());
        var folderUpdate = treeFolder.FirstOrDefault(x => x.Id == request.Id);
        _ = folderUpdate ?? throw new NotFoundException(_t["PaperFolder {0} Not Found.", request.Id]);

        folderUpdate.Update(request.Name, request.ParentId);

        await _paperFolderRepo.UpdateAsync(folderUpdate);

        return folder.Id;
    }
}
