

using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams.Specs;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.TeacherGroup;
using Mapster;

namespace FSH.WebApi.Application.Examination.Papers;
public class SharePaperRequest : IRequest<Guid>
{
    public Guid? UserId { get; set; }
    public Guid PaperId { get; set; }
    public Guid? GroupId { get; set; }
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanShare { get; set; }
}
public class PaperPermissionDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public UserDetailsDto? User { get; set; }
    public Guid PaperId { get; set; }
    public Guid? GroupTeacherId { get; set; }
    public GroupTeacherDto? GroupTeacher { get; set; }
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanShare { get; set; }
}

public class SharePaperRequestHandler : IRequestHandler<SharePaperRequest, Guid>
{
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;
    private readonly IRepositoryWithEvents<Paper> _paperRepo;
    private readonly IUserService _userService;
    private readonly IRepositoryWithEvents<PaperFolder> _paperFolderRepo;
    private readonly IRepository<PaperPermission> _paperPermissionRepo;
    public SharePaperRequestHandler(
                      ICurrentUser currentUser,
                             IStringLocalizer<SharePaperRequestHandler> t,
                                    IRepositoryWithEvents<Paper> paperRepo,
                                        IUserService userService,
                                        IRepositoryWithEvents<PaperFolder> paperFolderRepo,
                                        IRepository<PaperPermission> paperPermissionRepo)
    {
        _currentUser = currentUser;
        _t = t;
        _paperRepo = paperRepo;
        _userService = userService;
        _paperFolderRepo = paperFolderRepo;
        _paperPermissionRepo = paperPermissionRepo;
    }

    public async Task<DefaultIdType> Handle(SharePaperRequest request, CancellationToken cancellationToken)
    {
        var paper = await _paperRepo.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId), cancellationToken);
        _ = paper ?? throw new NotFoundException(_t["The Paper {0} Not Found", request.PaperId]);

        var currentUserId = _currentUser.GetUserId();
        if (!paper.CanShare(currentUserId))
        {
            throw new ForbiddenException(_t["You do not have permission to share this paper."]);
        }

        if (request.GroupId.HasValue)
        {
            var existingPermission = paper.PaperPermissions.FirstOrDefault(pp => pp.UserId == request.UserId);
            if (existingPermission != null)
            {
                existingPermission.SetPermission(request.CanView, request.CanAdd, request.CanUpdate, request.CanDelete, request.CanShare);
                await _paperPermissionRepo.UpdateAsync(existingPermission);
            }
            else
            {
                var newPermission = new PaperPermission(null, request.PaperId, request.GroupId, request.CanView, request.CanAdd, request.CanUpdate, request.CanDelete, request.CanShare);
                await _paperPermissionRepo.AddAsync(newPermission);
            }
        }

        if (request.UserId.HasValue)
        {
            var existingPermission = paper.PaperPermissions.FirstOrDefault(pp => pp.UserId == request.UserId);
            if (existingPermission != null)
            {
                existingPermission.SetPermission(request.CanView, request.CanAdd, request.CanUpdate, request.CanDelete, request.CanShare);
                await _paperPermissionRepo.UpdateAsync(existingPermission);
            }
            else
            {
                var newPermission = new PaperPermission(request.UserId, request.PaperId, null, request.CanView, request.CanAdd, request.CanUpdate, request.CanDelete, request.CanShare);
                await _paperPermissionRepo.AddAsync(newPermission);
            }
        }

        return paper.Id;
    }
}
