
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class SearchPaperFolderRequest : IRequest<PaperFolderTreeDto>
{
    public Guid? ParentId { get; set; }
    public string? Name { get; set; }
}

public class SearchPaperFolderRequestHandler : IRequestHandler<SearchPaperFolderRequest, PaperFolderTreeDto>
{
    public readonly IReadRepository<PaperFolder> _paperFolderRepo;
    public readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;
    private readonly IStringLocalizer _t;
    public SearchPaperFolderRequestHandler(IReadRepository<PaperFolder> paperFolderRepo, ICurrentUser currentUser, IUserService userService, IStringLocalizer<SearchPaperFolderRequestHandler> t)
    {
        _paperFolderRepo = paperFolderRepo;
        _currentUser = currentUser;
        _userService = userService;
        _t = t;
    }

    public async Task<PaperFolderTreeDto> Handle(SearchPaperFolderRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();
        var data = new List<PaperFolder>();
        var count = 0;

        if (!string.IsNullOrEmpty(request.Name))  // Search by name
        {
            // Find all parent IDs
            var parentIds = new List<Guid>();
            var treeFolder = await _paperFolderRepo.ListAsync(new PaperFolderTreeSpec());
            var myTreeFolder = treeFolder.Where(x => x.CreatedBy == currentUserId).ToList();
            if (request.ParentId.HasValue)       //Search in folder
            {
                parentIds.Add(request.ParentId.Value);
               
                var parentFolder = treeFolder.FirstOrDefault(x => x.Id == request.ParentId.Value);
                if (parentFolder != null)
                {
                    parentFolder.ChildPaperFolderIds(null, parentIds);
                }
                var spec = new PaperFolderBySearchSpec(request, currentUserId);
                data = await _paperFolderRepo.ListAsync(spec, cancellationToken);
                if (parentIds.Any())
                {
                    var nullableParentIds = parentIds.Select(id => (Guid?)id).ToList();
                    data = data.Where(x => nullableParentIds.Contains(x.ParentId) || nullableParentIds.Contains(x.Id)).ToList();
                }
            }       
            else        //Search in root folder
            {   
                data = myTreeFolder.Where(folder => string.IsNullOrEmpty(request.Name) ||folder.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }
        else    
        {
            var spec = new PaperFolderTreeBySearchSpec(currentUserId,request);
            data = await _paperFolderRepo.ListAsync(spec, cancellationToken);
            data = data.Where(x => x.ParentId == request.ParentId).ToList();
        }

        var dtos = new List<PaperFolderDto>();

        foreach (var folder in data)
        {
            var dto = await CustomMappings.MapPaperFolderAsync(folder, _userService, cancellationToken);
            var parents = folder.ListParents();
            dto.Parents = parents.Adapt<List<PaperFolderParentDto>>();
            if (dto.PaperFolderPermissions.Any())
            {
                foreach (var per in dto.PaperFolderPermissions)
                {
                    if (per.UserId.HasValue)
                    {
                        var user_permission = await _userService.GetAsync(per.UserId.ToString(), cancellationToken);
                        if (user_permission != null)
                        {
                            per.User = user_permission;
                        }
                    }
                }
            }
            dtos.Add(dto);
        }

        if (request.ParentId.HasValue)
        {
            var paperFolder = await _paperFolderRepo.FirstOrDefaultAsync(new PaperFolderByIdSpec(request.ParentId.Value), cancellationToken);
            _ = paperFolder ?? throw new NotFoundException(_t["PaperFolder {0} Not Found.", request.ParentId.Value]);

            return new PaperFolderTreeDto
            {
                Id = request.ParentId.Value,
                PaperFolderPermissions = paperFolder.PaperFolderPermissions.Adapt<List<PaperFolderPermissionDto>>(),
                PaperFolderChildrens = dtos,
                TotalPapers = paperFolder.CountPapers()
            };
        }
        else
        {
            return new PaperFolderTreeDto
            {
                PaperFolderChildrens = dtos
            };
        }
    }
}

public static class CustomMappings
{
    public static async Task<PaperFolderDto> MapPaperFolderAsync(PaperFolder paperFolder, IUserService userService, CancellationToken cancellationToken)
    {
        var dto = paperFolder.Adapt<PaperFolderDto>();

        // Lấy tên người tạo
        dto.CreatorName = await userService.GetFullName(paperFolder.CreatedBy);

        // Xử lý các mục con
        if (paperFolder.PaperFolderChildrens != null && paperFolder.PaperFolderChildrens.Any())
        {
            dto.PaperFolderChildrens = new List<PaperFolderDto>();
            foreach (var child in paperFolder.PaperFolderChildrens)
            {
                var childDto = await MapPaperFolderAsync(child, userService, cancellationToken);
                dto.PaperFolderChildrens.Add(childDto);
            }
        }

        return dto;
    }
}
