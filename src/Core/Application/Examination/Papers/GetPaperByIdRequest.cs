using FSH.WebApi.Application.Examination.Papers.Dtos;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Examination.Papers;
public class GetPaperByIdRequest : IRequest<PaperDto>
{
    public Guid Id { get; set; }
    public GetPaperByIdRequest(Guid id)
    {
        Id = id;
    }
}

public class GetPaperByIdRequestHandler : IRequestHandler<GetPaperByIdRequest, PaperDto>
{
    private readonly IRepository<Paper> _repository;
    private readonly IStringLocalizer _t;
    private readonly IUserService _userService;
    public GetPaperByIdRequestHandler(IRepository<Paper> repository, IStringLocalizer<GetPaperByIdRequestHandler> t, IUserService userService)
    {
        _repository = repository;
        _t = t;
        _userService = userService;
    }

    public async Task<PaperDto> Handle(GetPaperByIdRequest request, CancellationToken cancellationToken)
    {
        var spec = new PaperByIdSpec(request.Id);
        var paper = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
        _ = paper ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.Id]);

        var paperDto = paper.Adapt<PaperDto>();
        paperDto.CreatorName = await _userService.GetFullName(paper.CreatedBy);
        paperDto.TotalAttended = paper.GetTotalSubmissions();

        return paperDto;

    }
}
