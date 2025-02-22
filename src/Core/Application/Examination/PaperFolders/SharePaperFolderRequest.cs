﻿
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.Notifications;
using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.TeacherGroup;
using Mapster;


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
    private readonly IReadRepository<TeacherTeam> _teacherTeamRepo;
    private readonly IRepository<PaperFolderPermission> _paperFolderPermissionRepo;
    private readonly IMediator _mediator;
    private readonly INotificationService _notificationService;
    private readonly IRepository<GroupTeacher> _groupTeacherRepo;

    public SharePaperFolderRequestHandler(
        ICurrentUser currentUser,
        IStringLocalizer<SharePaperFolderRequestHandler> t,
        IRepositoryWithEvents<PaperFolder> paperFolderRepo,
        IReadRepository<TeacherTeam> teacherTeamRepo,
        IRepository<PaperFolderPermission> paperFolderPermissionRepo,
        IMediator mediator,
        INotificationService notificationService,
        IRepository<GroupTeacher> groupTeacherRepo
        )
    {
        _currentUser = currentUser;
        _t = t;
        _paperFolderRepo = paperFolderRepo;
        _teacherTeamRepo = teacherTeamRepo;
        _paperFolderPermissionRepo = paperFolderPermissionRepo;
        _mediator = mediator;
        _notificationService = notificationService;
        _groupTeacherRepo = groupTeacherRepo;
    }

    public async Task<DefaultIdType> Handle(SharePaperFolderRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();
        var folderTree = await _paperFolderRepo.ListAsync(new MyPaperFolderTreeSpec(currentUserId), cancellationToken);

           var folderParent = folderTree.FirstOrDefault(x => x.Id == request.FolderId)
            ?? throw new NotFoundException(_t["The Folder {0} Not Found", request.FolderId]);

        var folderChildrenIds = new List<Guid>();
        
        folderParent.ChildPaperFolderIds(null, folderChildrenIds);
        folderChildrenIds.Add(request.FolderId);

       
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

            var groupTeacher = await _groupTeacherRepo.FirstOrDefaultAsync(new GroupTeacherByIdSpec(request.GroupId.Value), cancellationToken)
               ?? throw new NotFoundException(_t["The Group {0} Not Found", request.GroupId.Value]);

            var groupMemberIds = groupTeacher.TeacherInGroups.Select(tig => tig.TeacherTeam.TeacherId);

            // Tạo và gửi thông báo cho các thành viên nhóm
            if (groupMemberIds.Any())
            {
                var notification = new BasicNotification
                {
                    Title = "Folder Shared with Group",
                    Message = $"Your group ({groupTeacher.Name}) has been granted access to the folder '{folderParent.Name}'.",
                    Label = BasicNotification.LabelType.Information,
                };

                await _notificationService.SendNotificationToUsers(groupMemberIds.Adapt<List<string>>(), notification, null, cancellationToken);
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

            var notification = new BasicNotification
            {
                Title = "Folder Shared with You",
                Message = $"You have been granted access to the folder '{folderParent.Name}'.",
                Label = BasicNotification.LabelType.Information,
            };

            await _notificationService.SendNotificationToUser(request.UserId.Value.ToString(), notification, null, cancellationToken);
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
