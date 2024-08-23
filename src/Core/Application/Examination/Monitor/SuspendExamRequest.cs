using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Monitor;

public class SuspendExamRequest : IRequest<DefaultIdType>
{
    public DefaultIdType PaperId { get; set; }
    public DefaultIdType? UserId { get; set; }
    public string? StudentEmail { get; set; }
    public string? Reason { get; set; }
}

public class SuspendExamRequestHandler : IRequestHandler<SuspendExamRequest, DefaultIdType>
{
    private readonly IRepository<Paper> _paperRepository;
    private readonly IRepository<SubmitPaper> _submitPaperRepository;
    private readonly IRepository<SubmitPaperLog> _submitPaperLogRepository;
    private readonly IUserService _userService;

    public SuspendExamRequestHandler(IRepository<Paper> paperRepository, IRepository<SubmitPaper> submitPaperRepository, IRepository<SubmitPaperLog> submitPaperLogRepository, IUserService userService)
    {
        _paperRepository = paperRepository;
        _submitPaperRepository = submitPaperRepository;
        _submitPaperLogRepository = submitPaperLogRepository;
        _userService = userService;
    }

    public async Task<DefaultIdType> Handle(SuspendExamRequest request, CancellationToken cancellationToken)
    {
        var spec = new PaperByIdSpec(request.PaperId);
        var paper = await _paperRepository.FirstOrDefaultAsync(spec, cancellationToken);
        _ = paper ?? throw new NotFoundException($"Paper {request.PaperId} Not Found.");

        if (!string.IsNullOrEmpty(request.StudentEmail))
        {
            var user = await _userService.GetUserDetailByEmailAsync(request.StudentEmail, cancellationToken);
            request.UserId = user.Id;

            if (user.Id == default)
            {
                throw new NotFoundException($"User {request.StudentEmail} Not Found.");
            }
        }

        // get submit paper by student id
        var sb = await _submitPaperRepository.FirstOrDefaultAsync(new SubmitPaperByStudentIdSpec(request.PaperId, request.UserId), cancellationToken);

        if (sb == null)
        {
            throw new NotFoundException($"Submit Paper Not Found.");
        }

        try
        {
            var log = new SubmitPaperLog
            {
                SubmitPaperId = sb.Id,
                IsSuspicious = true,
                SuspiciousReason = request.Reason,
            };

            _ = await _submitPaperLogRepository.AddAsync(log, cancellationToken);

            sb.Status = Domain.Examination.Enums.SubmitPaperStatus.Suspended;

            await _submitPaperRepository.UpdateAsync(sb, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ex.Message);
        }

        return sb.Id;
    }
}