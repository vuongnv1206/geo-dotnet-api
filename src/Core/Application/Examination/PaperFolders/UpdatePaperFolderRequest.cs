using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class UpdatePaperFolderRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid? ParentId { get; set; }
    public Guid? SubjectId { get; set;}
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
        var folder = await _paperFolderRepo.GetByIdAsync(request.Id);
        _ = folder ?? throw new NotFoundException(_t["Paper Folder {0} Not Found.", request.Id]);

        var updatedFolder = folder.Update(request.Name, request.ParentId, request.SubjectId);

        await _paperFolderRepo.UpdateAsync(updatedFolder);

        return folder.Id;
    }
}
