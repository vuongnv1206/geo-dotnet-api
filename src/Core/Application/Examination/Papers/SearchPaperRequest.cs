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
    public SearchPaperRequestHandler(IReadRepository<Paper> repository, IReadRepository<PaperFolder> paperFolderRepo)
    {
        _repository = repository;
        _paperFolderRepo = paperFolderRepo;
    }

    public async Task<List<PaperInListDto>> Handle(SearchPaperRequest request, CancellationToken cancellationToken)
    {
       

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
            var spec = new PaperBySearchSpec(parentIds, request.Name);
            data = await _repository.ListAsync(spec, cancellationToken);
        }
        else
        {
            var spec = new PaperBySearchSpec(null, request.Name);
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
        return dtos;
    }
}
