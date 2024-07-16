using FSH.WebApi.Application.Common.Models;
using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class SearchPaperFolderRequest : PaginationFilter, IRequest<PaginationResponse<PaperFolderDto>>
{
    public Guid? ParentId { get; set; }
}

public class SearchPaperFolderRequestHandler : IRequestHandler<SearchPaperFolderRequest, PaginationResponse<PaperFolderDto>>
{
    public readonly IReadRepository<PaperFolder> _paperFolderRepo;
    public readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;

    public SearchPaperFolderRequestHandler(IReadRepository<PaperFolder> paperFolderRepo, ICurrentUser currentUser, IUserService userService)
    {
        _paperFolderRepo = paperFolderRepo;
        _currentUser = currentUser;
        _userService = userService;
    }

    public async Task<PaginationResponse<PaperFolderDto>> Handle(SearchPaperFolderRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();
        var data = new List<PaperFolder>();
        var count = 0;
        if (!string.IsNullOrEmpty(request.Keyword))
        {
            // Find all parent IDs
            var parentIds = new List<Guid>();
            if (request.ParentId.HasValue)
            {
                parentIds.Add(request.ParentId.Value);
                var parentFolder = await _paperFolderRepo.FirstOrDefaultAsync(new PaperFolderByIdSpec(request.ParentId.Value));

                if (parentFolder != null)
                {
                    parentFolder.ChildPaperFolderIds(null, parentIds);
                }
            }
            var spec = new PaperFolderBySearchSpec(parentIds, request, currentUserId);
            count = await _paperFolderRepo.CountAsync(spec, cancellationToken);
            data = await _paperFolderRepo.ListAsync(spec, cancellationToken);
        }
        else
        {
            var spec = new PaperFolderTreeBySearchSpec(currentUserId,request);
            count = await _paperFolderRepo.CountAsync(spec, cancellationToken);
            data = await _paperFolderRepo.ListAsync(spec, cancellationToken);


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
        return new PaginationResponse<PaperFolderDto>(dtos, count, request.PageNumber, request.PageSize);
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
