using FSH.WebApi.Application.Common.Models;
using FSH.WebApi.Application.Examination.PaperFolders.Specs;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams.Specs;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.TeacherGroup;
using Mapster;
using System.Threading;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class SharePaperFolderRequest : IRequest<Guid>
{
    public List<Guid> UserIds { get; set; }
    public Guid FolderId { get; set; }
    public Guid? GroupId { get; set; }
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
}

public class PaperFolderPermissionSet
{
    public Guid UserId { get; set; }
    public Guid? GroupId { get; set; }
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
}

public class SharePaperFolderRequestHandler : IRequestHandler<SharePaperFolderRequest, Guid>
{
    private readonly IRepositoryWithEvents<PaperFolderPermission> _folderPermissionRepo;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;
    private readonly IRepositoryWithEvents<PaperFolder> _paperFolderRepo;
    public readonly IReadRepository<TeacherTeam> _teacherTeamRepo;

    public SharePaperFolderRequestHandler(
        IRepositoryWithEvents<PaperFolderPermission> folderPermissionRepo,
        ICurrentUser currentUser,
        IStringLocalizer<SharePaperFolderRequestHandler> t,
        IRepositoryWithEvents<PaperFolder> paperFolderRepo,
        IReadRepository<TeacherTeam> teacherTeamRepo)
    {
        _folderPermissionRepo = folderPermissionRepo;
        _currentUser = currentUser;
        _t = t;
        _paperFolderRepo = paperFolderRepo;
        _teacherTeamRepo = teacherTeamRepo;
    }

    public async Task<DefaultIdType> Handle(SharePaperFolderRequest request, CancellationToken cancellationToken)
    {
        var folder = await _paperFolderRepo.FirstOrDefaultAsync(new PaperFolderByIdSpec(request.FolderId), cancellationToken);
        _ = folder ?? throw new NotFoundException(_t["The Folder {0} Not Found", request.FolderId]);

        if (!folder.CanUpdate(_currentUser.GetUserId()))
        {
            throw new ForbiddenException(_t["You do not have permission to share this folder."]);
        }

        foreach (var userId in request.UserIds)
        {
            bool isExistUserInTeacherTeam = await _teacherTeamRepo.AnyAsync(new TeacherTeamByIdSpec(userId), cancellationToken);
            if (!isExistUserInTeacherTeam) continue;
            var permissionSet = new PaperFolderPermissionSet
            {
                UserId = userId,
                GroupId = request.GroupId,
                CanAdd = request.CanAdd,
                CanView = request.CanView,
                CanDelete = request.CanDelete,
                CanUpdate = request.CanUpdate,
            };

            await UpdatePermissionForFolder(folder, permissionSet, cancellationToken);
            foreach (var childFolder in folder.PaperFolderChildrens)
            {
                await UpdatePermissionForFolder(childFolder, permissionSet, cancellationToken);
            }
        }

        return folder.Id;
    }

    private async Task UpdatePermissionForFolder(
        PaperFolder folder,
        PaperFolderPermissionSet permissionSet,
        CancellationToken cancellationToken)
    {
        var permission = await _folderPermissionRepo
            .FirstOrDefaultAsync(new PaperFolderPermissionByFolderIdAndGroupIdAndUserIdSpec
            (folder.Id, permissionSet.UserId, permissionSet.GroupId));

        if (permission is null)
        {
            permission = new PaperFolderPermission(
                permissionSet.UserId,
                folder.Id,
                permissionSet.GroupId,
                permissionSet.CanView,
                permissionSet.CanAdd,
                permissionSet.CanUpdate,
                permissionSet.CanDelete);
            folder.AddPermission(permission);
            await _paperFolderRepo.UpdateAsync(folder, cancellationToken);
        }
        else
        {
            permission.SetPermissions(
                permissionSet.CanView,
                permissionSet.CanAdd,
                permissionSet.CanUpdate,
                permissionSet.CanDelete);
            await _folderPermissionRepo.UpdateAsync(permission, cancellationToken);
        }
    }
}
