﻿using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Domain.Question;
using MediatR;

namespace FSH.WebApi.Application.Questions;
public class ShareQuestionFolderRequest : IRequest<DefaultIdType>
{
    public DefaultIdType FolderId { get; set; }
    public List<DefaultIdType> UserIDs { get; set; } = new();
    public List<DefaultIdType> TeacherGroupIDs { get; set; } = new();
    public List<string> Emails { get; set; } = new();
    public List<string> Phones { get; set; } = new();
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanShare { get; set; }

}

public class ShareQuestionFolderRequestValidator : CustomValidator<ShareQuestionFolderRequest>
{
    public ShareQuestionFolderRequestValidator()
    {
        RuleFor(x => x.FolderId).NotEmpty();
        RuleFor(x => x.UserIDs).NotEmpty().When(x => x.Emails.Count == 0 && x.Phones.Count == 0 && x.TeacherGroupIDs.Count == 0);
        RuleFor(x => x.TeacherGroupIDs).NotEmpty().When(x => x.UserIDs.Count == 0 && x.Emails.Count == 0 && x.Phones.Count == 0);
        RuleFor(x => x.Emails).NotEmpty().When(x => x.UserIDs.Count == 0 && x.Phones.Count == 0 && x.TeacherGroupIDs.Count == 0);
        RuleFor(x => x.Phones).NotEmpty().When(x => x.UserIDs.Count == 0 && x.Emails.Count == 0 && x.TeacherGroupIDs.Count == 0);
    }
}

public class ShareQuestionFolderRequestHandler : IRequestHandler<ShareQuestionFolderRequest, Guid>
{
    private readonly IRepository<QuestionFolder> _repository;
    private readonly IRepository<QuestionFolderPermission> _permissionRepository;
    private readonly IUserService _userService;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;

    public ShareQuestionFolderRequestHandler(IRepository<QuestionFolder> repository, IRepository<QuestionFolderPermission> permissionRepository, IStringLocalizer<ShareQuestionFolderRequestHandler> localizer, ICurrentUser currentUser, IUserService userService)
    {
        _repository = repository;
        _permissionRepository = permissionRepository;
        _userService = userService;
        _t = localizer;
        _currentUser = currentUser;
    }

    private bool AllPermissionsFalse(ShareQuestionFolderRequest request)
    {
        return !request.CanView && !request.CanAdd && !request.CanUpdate && !request.CanDelete && !request.CanShare;
    }

    private bool AllPermissionsFalse(QuestionFolderPermission request)
    {
        return !request.CanView && !request.CanAdd && !request.CanUpdate && !request.CanDelete && !request.CanShare;
    }

    public async Task InheritPermissions(QuestionFolder folder, QuestionFolderPermission permission, CancellationToken cancellationToken)
    {

        foreach (var childFolder in folder.Children)
        {

            var childPermission = await _permissionRepository.FirstOrDefaultAsync(new QuestionFolderPermissionByFolderIdAndUserIdSpec(childFolder.Id, (DefaultIdType)permission.UserId), cancellationToken);

            if (childPermission == null)
            {
                childPermission = new QuestionFolderPermission(permission.UserId, permission.GroupTeacherId, childFolder.Id, permission.CanView, permission.CanAdd, permission.CanUpdate, permission.CanDelete, permission.CanShare);
                childFolder.Permissions.Add(childPermission);
                await _repository.UpdateAsync(childFolder, cancellationToken);
            }
            else
            {
                childPermission.SetPermissions(permission.CanView, permission.CanAdd, permission.CanUpdate, permission.CanDelete, permission.CanShare);
                await _permissionRepository.UpdateAsync(childPermission, cancellationToken);
            }

            // Recursively apply permissions to subfolders
            var childFolderWithChildren = await _repository.FirstOrDefaultAsync(new QuestionFolderByIdSpec(childFolder.Id), cancellationToken);
            await InheritPermissions(childFolderWithChildren, permission, cancellationToken);

        }
    }

    public async Task<Guid> Handle(ShareQuestionFolderRequest request, CancellationToken cancellationToken)
    {
        var folder = await _repository.FirstOrDefaultAsync(new QuestionFolderByIdSpec(request.FolderId), cancellationToken);

        _ = folder ?? throw new NotFoundException(_t["Folder {0} Not Found.", request.FolderId]);

        if (!folder.CanShare(_currentUser.GetUserId()))
        {
            throw new ForbiddenException(_t["You do not have permission to share this folder."]);
        }

        HashSet<Guid> userIds = new();
        if (request.UserIDs.Count > 0)
        {
            userIds.UnionWith(request.UserIDs);
        }

        HashSet<Guid> teacherGroupIds = new();
        if (request.TeacherGroupIDs.Count > 0)
        {
            teacherGroupIds.UnionWith(request.TeacherGroupIDs);
        }

        if (request.Emails.Count > 0)
        {
            foreach (string email in request.Emails)
            {
                var user = await _userService.GetUserDetailByEmailAsync(email, cancellationToken);
                if (user != null)
                {
                    userIds.Add(user.Id);
                }
            }
        }

        if (request.Phones.Count > 0)
        {
            foreach (string phone in request.Phones)
            {
                var user = await _userService.GetUserDetailByPhoneAsync(phone, cancellationToken);
                if (user != null)
                {
                    userIds.Add(user.Id);
                }
            }
        }

        foreach (var userId in userIds)
        {
            if (userId == Guid.Empty)
            {
                continue;
            }

            var permission = await _permissionRepository.FirstOrDefaultAsync(new QuestionFolderPermissionByFolderIdAndUserIdSpec(folder.Id, userId), cancellationToken);

            if (permission == null)
            {
                permission = new QuestionFolderPermission(userId, Guid.Empty, folder.Id, request.CanView, request.CanAdd, request.CanUpdate, request.CanDelete, request.CanShare);
                folder.Permissions.Add(permission);
                await _repository.UpdateAsync(folder, cancellationToken);
            }
            else
            {
                permission.SetPermissions(request.CanView, request.CanAdd, request.CanUpdate, request.CanDelete, request.CanShare);
                await _permissionRepository.UpdateAsync(permission, cancellationToken);
            }

            // Apply permissions to all descendants
            await InheritPermissions(folder, permission, cancellationToken);

        }

        foreach (var teacherGroupId in teacherGroupIds)
        {

            if (teacherGroupId == Guid.Empty)
            {
                continue;
            }

            var permission = await _permissionRepository.FirstOrDefaultAsync(new QuestionFolderPermissionByFolderIdAndTeacherGroupIdSpec(folder.Id, teacherGroupId), cancellationToken);

            if (permission == null)
            {
                permission = new QuestionFolderPermission(Guid.Empty, teacherGroupId, folder.Id, request.CanView, request.CanAdd, request.CanUpdate, request.CanDelete, request.CanShare);
                folder.Permissions.Add(permission);
                await _repository.UpdateAsync(folder, cancellationToken);
            }
            else
            {

                permission.SetPermissions(request.CanView, request.CanAdd, request.CanUpdate, request.CanDelete, request.CanShare);
                await _permissionRepository.UpdateAsync(permission, cancellationToken);
            }

            // Apply permissions to all descendants
            await InheritPermissions(folder, permission, cancellationToken);

        }

        return folder.Id;
    }

}
