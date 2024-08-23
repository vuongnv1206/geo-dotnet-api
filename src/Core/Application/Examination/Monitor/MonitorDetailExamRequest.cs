using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Monitor;
public class MonitorDetailExamRequest : PaginationFilter, IRequest<PaginationResponse<SubmitPaperLog>>
{
    public DefaultIdType SubmitPaperId { get; set; }
}

public class MonitorDetailExamRequestHandler : IRequestHandler<MonitorDetailExamRequest, PaginationResponse<SubmitPaperLog>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<Classes> _classRepo;
    private readonly IRepository<Paper> _paperRepository;
    private readonly IRepository<SubmitPaper> _submitPaperRepo;
    private readonly IRepository<SubmitPaperLog> _submitPaperLogRepository;

    private readonly IUserService _userService;
    public MonitorDetailExamRequestHandler(
        ICurrentUser currentUser,
        IRepository<Classes> classRepo,
        IRepository<Paper> paperRepo,
        IUserService userService,
        IRepository<SubmitPaper> submitPaperRepo,
        IRepository<SubmitPaperLog> submitPaperLogRepository)
    {
        _currentUser = currentUser;
        _classRepo = classRepo;
        _paperRepository = paperRepo;
        _userService = userService;
        _submitPaperRepo = submitPaperRepo;
        _submitPaperLogRepository = submitPaperLogRepository;
    }

    async Task<PaginationResponse<SubmitPaperLog>> IRequestHandler<MonitorDetailExamRequest, PaginationResponse<SubmitPaperLog>>.Handle(MonitorDetailExamRequest request, CancellationToken cancellationToken)
    {
        // check page number and page size
        if (request.PageNumber < 1)
        {
            request.PageNumber = 1;
        }

        if (request.PageSize < 1)
        {
            request.PageSize = int.MaxValue;
        }

        var spec = new SubmitPaperLogBySubmitPaperIdSpec(request.SubmitPaperId, request.PageNumber, request.PageSize);
        var listLog = await _submitPaperLogRepository.ListAsync(spec, cancellationToken);
        var specCount = new SubmitPaperLogBySubmitPaperIdSpec(request.SubmitPaperId);
        int total = await _submitPaperLogRepository.CountAsync(specCount, cancellationToken);
        var res = new PaginationResponse<SubmitPaperLog>(listLog, total, request.PageNumber, request.PageSize);
        return res;

    }
}