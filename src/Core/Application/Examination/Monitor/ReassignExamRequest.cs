using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Monitor;

public class ReassignExamRequest : IRequest<DefaultIdType>
{
    public DefaultIdType PaperId { get; set; }
    public DefaultIdType UserId { get; set; }
}

public class SubmitPaperByStudentIdSpec : Specification<SubmitPaper>, ISingleResultSpecification
{
    private readonly DefaultIdType _paperId;
    private readonly DefaultIdType? _userId;

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
    private readonly IUserService _userService;

    public ReassignExamRequestHandler(IRepository<Paper> paperRepository, IUserService userService)
    {
        _paperRepository = paperRepository;
        _userService = userService;
    }

    public async Task<DefaultIdType> Handle(ReassignExamRequest request, CancellationToken cancellationToken)
    {
        var spec = new PaperByIdSpec(request.PaperId);
        var paper = await _paperRepository.FirstOrDefaultAsync(spec, cancellationToken);
        _ = paper ?? throw new NotFoundException($"Paper {request.PaperId} Not Found.");

        // get submit paper by student id
        var sb = await _submitPaperRepository.FirstOrDefaultAsync(new SubmitPaperByStudentIdSpec(request.PaperId, request.UserId), cancellationToken);

        // reassign exam
        sb.canResume = true;

        try
        {
            await _submitPaperRepository.UpdateAsync(sb, cancellationToken);
            return sb.Id;
        }
        catch (Exception ex) { throw new BadRequestException($"Failed to reassign exam. {ex.Message}"); }

    }

}