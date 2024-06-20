

using FSH.WebApi.Application.TeacherGroup.TeacherTeams.Specs;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.TeacherGroup;
using Mapster;

namespace FSH.WebApi.Application.Examination.Papers;
public class SharePaperRequest : IRequest<Guid>
{
    public List<Guid> UserIds { get; set; }
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
    public Guid UserId { get; set; }
    public Guid PaperId { get; set; }
    public Guid? GroupId { get; set; }
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
    public SharePaperRequestHandler(
                      ICurrentUser currentUser,
                             IStringLocalizer<SharePaperRequestHandler> t,
                                    IRepositoryWithEvents<Paper> paperRepo)
    {
        _currentUser = currentUser;
        _t = t;
        _paperRepo = paperRepo;
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

        List<PaperPermission> permissionsToUpdate = new List<PaperPermission>();
        foreach (var userId in request.UserIds)
        {
            var existingPermission = paper.PaperPermissions.FirstOrDefault(pp => pp.UserId == userId);
            if (existingPermission != null)
            {
                permissionsToUpdate.Add(existingPermission);
            }
            else
            {
                var newPermission = new PaperPermission(userId, request.PaperId, request.GroupId, request.CanView, request.CanAdd, request.CanUpdate, request.CanDelete, request.CanShare);
                permissionsToUpdate.Add(newPermission);
            }
        }

        paper.UpdatePermissions(permissionsToUpdate);

        await _paperRepo.UpdateAsync(paper, cancellationToken);
        return paper.Id;
    }
}
