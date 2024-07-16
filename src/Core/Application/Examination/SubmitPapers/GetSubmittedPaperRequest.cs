using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.SubmitPapers.Dtos;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class GetSubmittedPaperRequest : PaginationFilter, IRequest<PaginationResponse<SubmitPaperDto>>
{
    public Guid PaperId { get; set; }
    public Guid? ClassId { get; set; }
}

public class GetSubmittedPaperRequestHandler : IRequestHandler<GetSubmittedPaperRequest, PaginationResponse<SubmitPaperDto>>
{
    private readonly IReadRepository<SubmitPaper> _repository;
    private readonly IRepository<Paper> _paperRepo;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;
    private readonly IUserService _userService;

    public GetSubmittedPaperRequestHandler(
        IReadRepository<SubmitPaper> repository,
        ICurrentUser currentUser,
        IStringLocalizer<GetSubmittedPaperRequestHandler> t,
        IRepository<Paper> paperRepo,
        IUserService userService)
    {
        _repository = repository;
        _currentUser = currentUser;
        _t = t;
        _paperRepo = paperRepo;
        _userService = userService;
    }

    public async Task<PaginationResponse<SubmitPaperDto>> Handle(GetSubmittedPaperRequest request, CancellationToken cancellationToken)
    {
        var paper = await _paperRepo.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId), cancellationToken)
            ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.PaperId]);

        var currentUserId = _currentUser.GetUserId();

        var spec = new SubmitPaperByPaperIdPaging(request, paper, currentUserId);
        var submitPapers = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
        foreach(var submitter in submitPapers.Data)
        {
            submitter.CreatorName = await _userService.GetFullName(submitter.CreatedBy);
        }

        return submitPapers;
    }
}
