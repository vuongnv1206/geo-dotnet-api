
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.TeacherGroup;


namespace FSH.WebApi.Application.Examination.PaperFolders;
public class SharePaperFolderRequest : IRequest<Guid>
{
    public Guid? UserId { get; set; }
    public Guid FolderId { get; set; }
    public Guid? GroupId { get; set; }
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanShare { get; set; }
}

public class PaperFolderPermissionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FolderId { get; set; }
    public Guid? GroupId { get; set; }
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanShare { get; set; }
}

public class SharePaperFolderRequestHandler : IRequestHandler<SharePaperFolderRequest, Guid>
{
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;
    private readonly IRepositoryWithEvents<PaperFolder> _paperFolderRepo;
    private readonly IReadRepository<TeacherTeam> _teacherTeamRepo;
    private readonly IRepository<PaperFolderPermission> _paperFolderPermissionRepo;
    private readonly IMediator _mediator;

    public SharePaperFolderRequestHandler(
        ICurrentUser currentUser,
        IStringLocalizer<SharePaperFolderRequestHandler> t,
        IRepositoryWithEvents<PaperFolder> paperFolderRepo,
        IReadRepository<TeacherTeam> teacherTeamRepo,
        IRepository<PaperFolderPermission> paperFolderPermissionRepo,
        IMediator mediator
        )
    {
        _currentUser = currentUser;
        _t = t;
        _paperFolderRepo = paperFolderRepo;
        _teacherTeamRepo = teacherTeamRepo;
        _paperFolderPermissionRepo = paperFolderPermissionRepo;
        _mediator = mediator;
    }

    public async Task<DefaultIdType> Handle(SharePaperFolderRequest request, CancellationToken cancellationToken)
    {
        var folderTree = await _paperFolderRepo.ListAsync(new PaperFolderTreeSpec(), cancellationToken);
           var folderParent = folderTree.FirstOrDefault(x => x.Id == request.FolderId)
            ?? throw new NotFoundException(_t["The Folder {0} Not Found", request.FolderId]);

        var folderChildrenIds = new List<Guid>();
        
        folderParent.ChildPaperFolderIds(null, folderChildrenIds);
        folderChildrenIds.Add(request.FolderId);

        var currentUserId = _currentUser.GetUserId();
        if (!folderParent.CanShare(currentUserId))
        {
            throw new ForbiddenException(_t["You do not have permission to share this folder."]);
        }

        if (request.GroupId.HasValue)
        {
            foreach (var folderChildrenId in folderChildrenIds)
            {
                var folderChildren = await _paperFolderRepo.FirstOrDefaultAsync(new PaperFolderByIdSpec(folderChildrenId), cancellationToken)
                    ?? throw new NotFoundException(_t["The Folder {0} Not Found", folderChildrenId]);

                var existingPermission = folderChildren.PaperFolderPermissions.FirstOrDefault(pp => pp.GroupTeacherId == request.GroupId);

                if (existingPermission != null)
                {
                    existingPermission.SetPermissions(request.CanView, request.CanAdd, request.CanUpdate, request.CanDelete, request.CanShare);
                    await _paperFolderPermissionRepo.UpdateAsync(existingPermission, cancellationToken);
                }
                else
                {
                    var newPermission = new PaperFolderPermission(null, folderChildrenId, request.GroupId, request.CanView, request.CanAdd, request.CanUpdate, request.CanDelete, request.CanShare);
                    await _paperFolderPermissionRepo.AddAsync(newPermission);
                }
            }
        }

        if (request.UserId.HasValue)
        {
            foreach (var folderChildrenId in folderChildrenIds)
            {
                var folderChildren = await _paperFolderRepo.FirstOrDefaultAsync(new PaperFolderByIdSpec(folderChildrenId), cancellationToken)
                   ?? throw new NotFoundException(_t["The Folder {0} Not Found", folderChildrenId]);
                var existingPermission = folderChildren.PaperFolderPermissions.FirstOrDefault(pp => pp.UserId == request.UserId);

                if (existingPermission != null)
                {
                    existingPermission.SetPermissions(request.CanView, request.CanAdd, request.CanUpdate, request.CanDelete, request.CanShare);
                    await _paperFolderPermissionRepo.UpdateAsync(existingPermission, cancellationToken);

                }
                else
                {
                    var newPermission = new PaperFolderPermission(request.UserId, folderChildrenId, null, request.CanView, request.CanAdd, request.CanUpdate, request.CanDelete, request.CanShare);
                    await _paperFolderPermissionRepo.AddAsync(newPermission);
                }
            }
        }

        foreach (var folderChildrenId in folderChildrenIds)
        {
            var folderChildren = await _paperFolderRepo.FirstOrDefaultAsync(new PaperFolderByIdSpec(folderChildrenId), cancellationToken)
                ?? throw new NotFoundException(_t["The Folder {0} Not Found", folderChildrenId]);

            foreach (var paper in folderChildren.Papers)
            {
               var sharePaperRequest = new SharePaperRequest
               {
                   PaperId = paper.Id,
                   UserId = request.UserId,
                   GroupId = request.GroupId,
                   CanView = request.CanView,
                   CanAdd = request.CanAdd,
                   CanUpdate = request.CanUpdate,
                   CanDelete = request.CanDelete,
                   CanShare = request.CanShare
               };
                await _mediator.Send(sharePaperRequest, cancellationToken);
            }
        }
        return folderParent.Id;
    }
}
