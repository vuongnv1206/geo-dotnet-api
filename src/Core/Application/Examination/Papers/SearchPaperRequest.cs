using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Examination.Papers;
public class SearchPaperRequest : IRequest<List<PaperInListDto>>
{
    public Guid? PaperFolderId { get; set; }
    public string? Name { get; set; }
}

public class SearchPaperRequestHandler : IRequestHandler<SearchPaperRequest, List<PaperInListDto>>
{
    private readonly IReadRepository<Paper> _repository;
    public readonly IReadRepository<PaperFolder> _paperFolderRepo;
    public readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;
    public SearchPaperRequestHandler(IReadRepository<Paper> repository, IReadRepository<PaperFolder> paperFolderRepo, ICurrentUser currentUser, IUserService userService)
    {
        _repository = repository;
        _paperFolderRepo = paperFolderRepo;
        _currentUser = currentUser;
        _userService = userService;
    }

    public async Task<List<PaperInListDto>> Handle(SearchPaperRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();

        var data = new List<Paper>();
        var parentIds = new List<Guid>();
        var count = 0;

        //If search by name
        if (!string.IsNullOrEmpty(request.Name))
        {
            //If search by name in folder
            if (request.PaperFolderId.HasValue)
            {
                parentIds.Add(request.PaperFolderId.Value);
                var parentFolder = await _paperFolderRepo.FirstOrDefaultAsync(new PaperFolderByIdSpec(request.PaperFolderId.Value));
                if (parentFolder != null)
                {
                    parentFolder.ChildPaperFolderIds(null, parentIds);
                }
                var spec = new PaperBySearchSpec(parentIds, request);
                count = await _repository.CountAsync(spec, cancellationToken);
                data = await _repository.ListAsync(spec, cancellationToken);
            }
            else  //If search by name in root
            {
                var spec = new PaperBySearchSpec(null, request);
                count = await _repository.CountAsync(spec, cancellationToken);
                data = await _repository.ListAsync(spec, cancellationToken);
            }
        }
        else
        {
            //If specific folder
            if (request.PaperFolderId.HasValue)
            {
                parentIds.Add(request.PaperFolderId.Value);
                var spec = new PaperBySearchSpec(parentIds, request);
                data = await _repository.ListAsync(spec, cancellationToken);
            }//If root
            else
            {
                var spec = new PaperBySearchSpec(null, request);
                data = await _repository.ListAsync(spec, cancellationToken);
            }
        }

         

        var dtos = new List<PaperInListDto>();
        foreach (var paper in data)
        {
            var dto = paper.Adapt<PaperInListDto>();
            if (paper.PaperFolderId != null)
            {
                var parents = paper.PaperFolder.ListParents();
                dto.Parents = parents.Adapt<List<PaperFolderParentDto>>();
            }
            dto.CreatorName = await _userService.GetFullName(paper.CreatedBy);
            dtos.Add(dto);
        }
        return dtos;

    }
}
