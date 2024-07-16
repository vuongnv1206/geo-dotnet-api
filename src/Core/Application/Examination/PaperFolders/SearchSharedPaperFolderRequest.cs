

using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.TeacherGroup;
using Mapster;
using System.Linq;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class SearchSharedPaperFolderRequest : IRequest<List<PaperFolderDto>>
{
    public Guid? ParentId { get; set; }
    public string? Name { get; set; }
}

public class SearchSharedPaperFolderRequestHandler : IRequestHandler<SearchSharedPaperFolderRequest, List<PaperFolderDto>>
{
    private readonly IReadRepository<PaperFolder> _paperFolderRepo;
    private readonly IReadRepository<GroupTeacher> _groupTeacherRepo;
    private readonly IReadRepository<PaperFolderPermission> _paperFolderPermissionRepo;
    private readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;

    public SearchSharedPaperFolderRequestHandler(IReadRepository<PaperFolder> paperFolderRepo, IReadRepository<GroupTeacher> groupTeacherRepo, IReadRepository<PaperFolderPermission> paperFolderPermissionRepo, ICurrentUser currentUser, IUserService userService)
    {
        _paperFolderRepo = paperFolderRepo;
        _groupTeacherRepo = groupTeacherRepo;
        _paperFolderPermissionRepo = paperFolderPermissionRepo;
        _currentUser = currentUser;
        _userService = userService;
    }

    public async Task<List<PaperFolderDto>> Handle(SearchSharedPaperFolderRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();

        // Lấy các nhóm mà người dùng hiện tại thuộc về
        var accessibleGroups = await _groupTeacherRepo.ListAsync(new GroupTeacherByUserSpec(currentUserId), cancellationToken);
        var groupIds = accessibleGroups.Select(g => g.Id).ToList();


        var accessibleFolders = await _paperFolderPermissionRepo.ListAsync(new PaperFolderPermissionByUserOrGroupSpec(currentUserId,groupIds), cancellationToken);
        var accessibleFolderIds = accessibleFolders.Select(p => p.FolderId).Distinct();

        var data = new List<PaperFolder>();

        if (!string.IsNullOrEmpty(request.Name))
        {
            var spec = new AccessibleFoldersWithChildrenSpec(accessibleFolderIds, request);
            data = await _paperFolderRepo.ListAsync(spec, cancellationToken);

        }
        else
        {
            var specRoot = new AccessibleFoldersTreeSpec(accessibleFolderIds, request);
            data = await _paperFolderRepo.ListAsync(specRoot, cancellationToken);
            if (!request.ParentId.HasValue)
            {
                //if root thì mới thêm các con mà được share
                var specChildren = new SearchSharedPaperFolderChildrenSpec(accessibleFolderIds);
                data.AddRange(await _paperFolderRepo.ListAsync(specChildren, cancellationToken));
            }
        }

        var dtos = new List<PaperFolderDto>();

        foreach (var folder in data)
        {
            var dto = await CustomMappings.MapPaperFolderAsync(folder, _userService, cancellationToken);
            var parents = folder.ListAccessibleParents(accessibleFolderIds);
            dto.Parents = parents.Adapt<List<PaperFolderParentDto>>();
            dtos.Add(dto);
        }
        return dtos;
    }
}
  
