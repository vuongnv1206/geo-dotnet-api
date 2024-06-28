using FSH.WebApi.Application.Common.Models;
using FSH.WebApi.Application.Examination.PaperFolders.Specs;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams.Specs;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.TeacherGroup;
using Mapster;
using MapsterMapper;
using MediatR;
using System.Threading;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class SharePaperFolderRequest : IRequest<Guid>
{
    public List<Guid>? UserIds { get; set; }
    public Guid FolderId { get; set; }
    public List<Guid>? GroupIds { get; set; }
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanShare { get; set; }
}

public class PaperFolderPermissionDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public UserDetailsDto? User { get; set; }
    public Guid FolderId { get; set; }
    public Guid? GroupTeacherId { get; set; }
    public GroupTeacherDto? GroupTeacher { get; set; }
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
    public readonly IReadRepository<TeacherTeam> _teacherTeamRepo;

    public SharePaperFolderRequestHandler(
        ICurrentUser currentUser,
        IStringLocalizer<SharePaperFolderRequestHandler> t,
        IRepositoryWithEvents<PaperFolder> paperFolderRepo,
        IReadRepository<TeacherTeam> teacherTeamRepo)
    {
        _currentUser = currentUser;
        _t = t;
        _paperFolderRepo = paperFolderRepo;
        _teacherTeamRepo = teacherTeamRepo;
    }

    public async Task<DefaultIdType> Handle(SharePaperFolderRequest request, CancellationToken cancellationToken)
    {
        var folder = await _paperFolderRepo.FirstOrDefaultAsync(new PaperFolderByIdSpec(request.FolderId), cancellationToken)
            ?? throw new NotFoundException(_t["The Folder {0} Not Found", request.FolderId]);

        var currentUserId = _currentUser.GetUserId();
        if (!folder.CanShare(currentUserId))
        {
            throw new ForbiddenException(_t["You do not have permission to share this folder."]);
        }

        var permissionsToUpdate = new List<PaperFolderPermission>();

        if (request.GroupIds.Any())
        {
            foreach (var groupId in request.GroupIds)
            {
                var existingPermission = folder.PaperFolderPermissions
                    .FirstOrDefault(pp => pp.GroupTeacherId == groupId);

                if (existingPermission != null)
                {
                    existingPermission.SetPermissions(request.CanView, request.CanAdd, request.CanUpdate, request.CanDelete, request.CanShare);
                    permissionsToUpdate.Add(existingPermission);
                }
                else
                {
                    var newPermission = new PaperFolderPermission(null, request.FolderId, groupId, request.CanView, request.CanAdd, request.CanUpdate, request.CanDelete, request.CanShare);
                    permissionsToUpdate.Add(newPermission);
                }
            }
        }

        if (request.UserIds.Any())
        {
            foreach (var userId in request.UserIds)
            {
                var existingPermission = folder.PaperFolderPermissions
                    .FirstOrDefault(pp => pp.UserId == userId);

                if (existingPermission != null)
                {
                    existingPermission.SetPermissions(request.CanView, request.CanAdd, request.CanUpdate, request.CanDelete, request.CanShare);
                    permissionsToUpdate.Add(existingPermission);
                }
                else
                {
                    var newPermission = new PaperFolderPermission(userId, request.FolderId, null, request.CanView, request.CanAdd, request.CanUpdate, request.CanDelete, request.CanShare);
                    permissionsToUpdate.Add(newPermission);
                }
            }
        }

        folder.UpdatePermissions(permissionsToUpdate);

        await _paperFolderRepo.UpdateAsync(folder, cancellationToken);

        return folder.Id;
    }
}
