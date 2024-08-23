using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.SubmitPapers;
using FSH.WebApi.Application.Extensions;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Examination.Reviews;
public class GetLastResultExamRequest : IRequest<LastResultExamDto>
{
    public Guid PaperId { get; set; }
    public Guid UserId { get; set; }
    public Guid SubmitPaperId { get; set; }

}

public class GetLastResultExamRequestHandler : IRequestHandler<GetLastResultExamRequest, LastResultExamDto>
{
    private readonly IReadRepository<SubmitPaper> _repositorySubmitPaper;
    private readonly IUserService _userService;
    private readonly IReadRepository<Paper> _repositoryPaper;
    private readonly IStringLocalizer _t;
    private readonly ISubmmitPaperService _submmitPaperService;

    public GetLastResultExamRequestHandler(
        IReadRepository<SubmitPaper> repositorySubmitPaper,
        IUserService userService,
        IReadRepository<Paper> repositoryPaper,
        IStringLocalizer<GetLastResultExamRequestHandler> t,
        ISubmmitPaperService submmitPaperService)
    {
        _repositorySubmitPaper = repositorySubmitPaper;
        _userService = userService;
        _repositoryPaper = repositoryPaper;
        _t = t;
       _submmitPaperService = submmitPaperService;
    }

    public async Task<LastResultExamDto> Handle(GetLastResultExamRequest request, CancellationToken cancellationToken)
    {
        var examResultDto = await _submmitPaperService.GetLastResultExamAsync(request.PaperId, request.UserId, request.SubmitPaperId, cancellationToken);
        return examResultDto;
    }
}
