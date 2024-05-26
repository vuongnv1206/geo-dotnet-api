using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;
using Mapster;
using MapsterMapper;

namespace FSH.WebApi.Application.Examination.Papers;
public class SearchPaperRequest : IRequest<List<PaperInListDto>>
{
    public Guid? PaperFolderId { get; set; }
    public string? Name { get; set; }
}

public class SearchPaperRequestHandler : IRequestHandler<SearchPaperRequest, List<PaperInListDto>>
{
    private readonly IReadRepository<Paper> _repository;
    private readonly IUserService _userService;
    public SearchPaperRequestHandler(IReadRepository<Paper> repository, IUserService userService)
    {
        _repository = repository;
        _userService = userService;
    }

    public async Task<List<PaperInListDto>> Handle(SearchPaperRequest request, CancellationToken cancellationToken)
    {
        var spec = new PaperBySearchSpec(request);
        var data = await _repository.ListAsync(spec, cancellationToken);

        var paperInListDtos = data.Adapt<List<PaperInListDto>>();
        foreach (var paperInListDto in paperInListDtos)
        {
            paperInListDto.CreatorName = await _userService.GetFullName(paperInListDto.CreatedBy);
        }
        return paperInListDtos;
    }
}
