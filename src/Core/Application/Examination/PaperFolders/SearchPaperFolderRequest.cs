using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class SearchPaperFolderRequest : IRequest<List<PaperFolderDto>>
{
    public Guid? ParentId { get; set; }
    public string? Name { get; set; }
}

public class SearchPaperFolderRequestHandler : IRequestHandler<SearchPaperFolderRequest, List<PaperFolderDto>>
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

    public async Task<List<PaperFolderDto>> Handle(SearchPaperFolderRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();
        var data = new List<PaperFolder>();

        if (!string.IsNullOrEmpty(request.Name))
        {
            var spec = new PaperFolderBySearchWithNameSpec(request.Name, currentUserId);
            data = await _paperFolderRepo.ListAsync(spec, cancellationToken);
        }
        else if (request.ParentId == null || request.ParentId.HasValue)
        {
            var spec = new PaperFolderBySearchWithParentIdSpec(currentUserId);
            data = await _paperFolderRepo.ListAsync(spec, cancellationToken);
            data = data.Where(x => x.ParentId == request.ParentId).ToList();
        }


        var dtos = new List<PaperFolderDto>();
        foreach (var folder in data)
        {
            var dto = await CustomMappings.MapPaperFolderAsync(folder, _userService, cancellationToken);
            dtos.Add(dto);
        }

        return dtos;

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

}