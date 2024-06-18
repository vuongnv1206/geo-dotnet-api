

using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class SearchSharedPaperFolderRequest : PaginationFilter, IRequest<PaginationResponse<PaperFolderDto>>
{
    public Guid? ParentId { get; set; }
}

public class SearchSharedPaperFolderRequestHandler : IRequestHandler<SearchSharedPaperFolderRequest, PaginationResponse<PaperFolderDto>>
{
    private readonly IReadRepository<PaperFolder> _paperFolderRepo;
    private readonly IReadRepository<PaperFolderPermission> _paperFolderPermissionRepo;
    private readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;

    public SearchSharedPaperFolderRequestHandler(IReadRepository<PaperFolder> paperFolderRepo, ICurrentUser currentUser, IUserService userService, IReadRepository<PaperFolderPermission> paperFolderPermissionRepo)
    {
        _paperFolderRepo = paperFolderRepo;
        _currentUser = currentUser;
        _userService = userService;
        _paperFolderPermissionRepo = paperFolderPermissionRepo;
    }

    public async Task<PaginationResponse<PaperFolderDto>> Handle(SearchSharedPaperFolderRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();

        var accessibleFolders = await _paperFolderPermissionRepo.ListAsync(new PaperFolderPermissionByUserSpec(currentUserId,request), cancellationToken);
        var accessibleFolderIds = accessibleFolders.Select(p => p.FolderId).Distinct();

        var data = new List<PaperFolder>();

        if (!string.IsNullOrEmpty(request.Keyword))
        {
            // Find all parent IDs
            var parentIds = new List<Guid>();
            if (request.ParentId.HasValue)
            {
                parentIds.Add(request.ParentId.Value);
                var parentFolder = await _paperFolderRepo.GetByIdAsync(request.ParentId.Value);
                if (parentFolder != null)
                {
                    parentFolder.ChildPaperFolderIds(null, parentIds);
                }
            }

            var searchFolderIds = accessibleFolderIds.Intersect(parentIds).ToList();
            var spec = new AccessibleFoldersWithChildrenSpec(searchFolderIds,request);
            data = await _paperFolderRepo.ListAsync(spec, cancellationToken);
        }
        else
        {
            var spec = new AccessibleFoldersTreeSpec(accessibleFolderIds,request);
            data = await _paperFolderRepo.ListAsync(spec, cancellationToken);
            data = data.Where(x => x.ParentId == request.ParentId).ToList();
        }

        var dtos = new List<PaperFolderDto>();

        foreach (var folder in data)
        {
            var dto = await CustomMappings.MapPaperFolderAsync(folder, _userService, cancellationToken);
            var parents = folder.ListParents();
            dto.Parents = parents.Adapt<List<PaperFolderParentDto>>();
            dtos.Add(dto);
        }
        return new PaginationResponse<PaperFolderDto>(dtos, dtos.Count(), request.PageNumber, request.PageSize);
    }
}
  
