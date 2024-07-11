


using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Assignments.AssignmentStudent;
public class MarkAssignmentRequest : IRequest<Guid>
{
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public float Score { get; set; }
}

public class MarkAssignmentRequestHandler : IRequestHandler<MarkAssignmentRequest, Guid>
{
    private readonly IRepository<Assignment> _assignmentRepo;
    private readonly IRepository<Student> _studentRepo;
    private readonly IStringLocalizer<MarkAssignmentRequestHandler> _t;

    public MarkAssignmentRequestHandler(IRepository<Assignment> assignmentRepo, IRepository<Student> studentRepo, IStringLocalizer<MarkAssignmentRequestHandler> t)
    {
        _assignmentRepo = assignmentRepo;
        _studentRepo = studentRepo;
        _t = t;
    }

    public async Task<Guid> Handle(MarkAssignmentRequest request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepo.FirstOrDefaultAsync(new AssignmentByIdSpec(request.AssignmentId));
        _ = assignment ?? throw new NotFoundException(_t["Assignment {0} Not  Found.", request.AssignmentId]);

        assignment.MarkScoreForSubmission(request.StudentId, request.Score);
        await _assignmentRepo.UpdateAsync(assignment);

        return assignment.Id;
    }
}
