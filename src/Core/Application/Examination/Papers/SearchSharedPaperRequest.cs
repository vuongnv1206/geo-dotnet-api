

using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Examination.Papers.Specs;
using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Examination.Papers;
public class SearchSharedPaperRequest : PaginationFilter, IRequest<PaginationResponse<PaperInListDto>>
{
    public Guid? PaperFolderId { get; set; }
}

public class SearchSharedPaperRequestHandler : IRequestHandler<SearchSharedPaperRequest, PaginationResponse<PaperInListDto>>
{
    private readonly IReadRepository<Paper> _repository;
    public readonly IReadRepository<PaperFolder> _paperFolderRepo;
    private readonly IReadRepository<PaperPermission> _paperPermissionRepo;

    private readonly ICurrentUser _currentUser;

    public SearchSharedPaperRequestHandler(IReadRepository<Paper> repository, IReadRepository<PaperFolder> paperFolderRepo, IReadRepository<PaperPermission> paperPermissionRepo, ICurrentUser currentUser)
    {
        _repository = repository;
        _paperFolderRepo = paperFolderRepo;
        _paperPermissionRepo = paperPermissionRepo;
        _currentUser = currentUser;
    }

    public async Task<PaginationResponse<PaperInListDto>> Handle(SearchSharedPaperRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();

        var accessiblePapers = await _paperPermissionRepo.ListAsync(new PaperPermissionByUserSpec(currentUserId),cancellationToken);
        var accessiblePaperIds = accessiblePapers.Select(p => p.PaperId).Distinct();


        var data = new List<Paper>();
        var parentIds = new List<Guid>();
        if (request.PaperFolderId.HasValue)
        {
            parentIds.Add(request.PaperFolderId.Value);
            var parentFolder = await _paperFolderRepo.GetByIdAsync(request.PaperFolderId.Value);
            if (parentFolder != null)
            {
                parentFolder.ChildPaperFolderIds(null, parentIds);
            }
            var spec = new AccessiblePapersSpec(parentIds,accessiblePaperIds, request);
            data = await _repository.ListAsync(spec, cancellationToken);
        }
        else
        {
            var spec = new AccessiblePapersSpec(null, accessiblePaperIds, request);
            data = await _repository.ListAsync(spec, cancellationToken);
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
            dtos.Add(dto);
        }
        return new PaginationResponse<PaperInListDto>(dtos, dtos.Count(), request.PageNumber, request.PageSize);

    }
}