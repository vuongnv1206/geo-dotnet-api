using FSH.WebApi.Application.Examination.SubmitPapers;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Examination.Papers.ByStudents;
public class GetPaperByIdRoleStudentRequest : IRequest<PaperStudentDto>
{
    public Guid Id { get; set; }
    public GetPaperByIdRoleStudentRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class GetPaperByIdRoleStudentRequestHandler : IRequestHandler<GetPaperByIdRoleStudentRequest, PaperStudentDto>
{
    private readonly IRepository<Paper> _repository;
    private readonly IStringLocalizer _t;
    private readonly IUserService _userService;

    public GetPaperByIdRoleStudentRequestHandler(
        IRepository<Paper> repository,
        IStringLocalizer<GetPaperByIdRoleStudentRequestHandler> t,
        IUserService userService)
    {
        _repository = repository;
        _t = t;
        _userService = userService;
    }

    public async Task<PaperStudentDto> Handle(GetPaperByIdRoleStudentRequest request, CancellationToken cancellationToken)
    {
        var spec = new PaperByIdSpec(request.Id);
        var paper = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
        _ = paper ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.Id]);

        var paperDto = paper.Adapt<PaperStudentDto>();
        paperDto.CreatorName = await _userService.GetFullName(paper.CreatedBy);

        return paperDto;
    }
}
