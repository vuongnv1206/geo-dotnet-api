using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Examination.Enums;

namespace FSH.WebApi.Application.Examination.Monitor;

public class ReassignExamRequest : IRequest<DefaultIdType>
{
    public DefaultIdType PaperId { get; set; }
    public DefaultIdType? UserId { get; set; }
    public string? StudentEmail { get; set; }
    public string? Reason { get; set; }
}

public class SubmitPaperByStudentIdSpec : Specification<SubmitPaper>, ISingleResultSpecification
{
    public SubmitPaperByStudentIdSpec(DefaultIdType paperId, DefaultIdType studentId)
    {
        _ = Query.Where(p => p.PaperId == paperId && p.CreatedBy == studentId);
    }

    public SubmitPaperByStudentIdSpec(DefaultIdType paperId, DefaultIdType? userId)
    {
        _ = Query.Where(p => p.PaperId == paperId && p.CreatedBy == userId);
    }
}

public class ReassignExamRequestHandler : IRequestHandler<ReassignExamRequest, DefaultIdType>
{
    private readonly IRepository<Paper> _paperRepository;
    private readonly IRepository<SubmitPaper> _submitPaperRepository;
    private readonly IRepository<SubmitPaperLog> _submitPaperLogRepository;
    private readonly IUserService _userService;

    public ReassignExamRequestHandler(IRepository<Paper> paperRepository, IRepository<SubmitPaper> submitPaperRepository, IRepository<SubmitPaperLog> submitPaperLogRepository, IUserService userService)
    {
        _paperRepository = paperRepository;
        _submitPaperRepository = submitPaperRepository;
        _submitPaperLogRepository = submitPaperLogRepository;
        _userService = userService;
    }

    public async Task<DefaultIdType> Handle(ReassignExamRequest request, CancellationToken cancellationToken)
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
            throw new NotFoundException($"Submit Paper {request.PaperId} Not Found.");
        }

        if (sb.Status != SubmitPaperStatus.Doing)
        {
            throw new BadRequestException($"Cannot reassign exam. The exam is not in progress.");
        }

        // reassign exam
        sb.canResume = true;

        try
        {
            await _submitPaperRepository.UpdateAsync(sb, cancellationToken);

            var log = new SubmitPaperLog
            {
                SubmitPaperId = sb.Id,
                ReassignLog = request.Reason,
            };

            _ = await _submitPaperLogRepository.AddAsync(log, cancellationToken);

            return sb.Id;
        }
        catch (Exception ex) { throw new BadRequestException($"Failed to reassign exam. {ex.Message}"); }

    }

}