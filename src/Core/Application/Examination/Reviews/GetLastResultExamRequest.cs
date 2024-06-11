using FSH.WebApi.Application.Extensions;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Examination.Reviews;
public class GetLastResultExamRequest : IRequest<LastResultExamDto>
{
    public Guid PaperId { get; set; }
    public Guid UserId { get; set;}
    public Guid SubmitPaperId { get; set; }

}

public class GetLastResultExamRequestHandler : IRequestHandler<GetLastResultExamRequest, LastResultExamDto>
{
    private readonly IReadRepository<SubmitPaper> _repositorySubmitPaper;
    private readonly IUserService _userService;
    private readonly IReadRepository<Paper> _repositoryPaper;
    private readonly IStringLocalizer _t;

    public GetLastResultExamRequestHandler(IReadRepository<SubmitPaper> repositorySubmitPaper, IUserService userService, IReadRepository<Paper> repositoryPaper, IStringLocalizer<GetLastResultExamRequestHandler> t)
    {
        _repositorySubmitPaper = repositorySubmitPaper;
        _userService = userService;
        _repositoryPaper = repositoryPaper;
        _t = t;
    }

    public async Task<LastResultExamDto> Handle(GetLastResultExamRequest request, CancellationToken cancellationToken)
    {
        var spec = new ExamResultSpec(request.SubmitPaperId,request.PaperId, request.UserId);
        var submitPaper = await _repositorySubmitPaper.FirstOrDefaultAsync(spec,cancellationToken);

        var student = await _userService.GetAsync(request.UserId.ToString(),cancellationToken);

        _ = submitPaper ?? throw new NotFoundException(_t["SubmitPaper Not Found."]);

    
       
        var examResultDto = submitPaper.Adapt<LastResultExamDto>();
        examResultDto.Student = student;



        return examResultDto;
    }
}
