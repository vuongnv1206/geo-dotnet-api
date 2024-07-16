using FSH.WebApi.Domain.Assignment;

namespace FSH.WebApi.Application.Assignments.AssignmentStudent;
public class UpdateStatusSubmitAssignmentRequest : IRequest<Guid>
{
    public Guid AssignmentId { get; set; }
    public SubmitAssignmentStatus Status { get; set; }
}

public class UpdateStatusSubmitAssignmentRequestHandler : IRequestHandler<UpdateStatusSubmitAssignmentRequest, Guid>
{
    private readonly IRepository<Assignment> _assignmentRepo;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer<UpdateStatusSubmitAssignmentRequestHandler> _t;

    public UpdateStatusSubmitAssignmentRequestHandler(
        IRepository<Assignment> assignmentRepo,
        ICurrentUser currentUser,
        IStringLocalizer<UpdateStatusSubmitAssignmentRequestHandler> t)
    {
        _assignmentRepo = assignmentRepo;
        _currentUser = currentUser;
        _t = t;
    }

    public async Task<Guid> Handle(UpdateStatusSubmitAssignmentRequest request, CancellationToken cancellationToken)
    {

        var assignment = await _assignmentRepo.FirstOrDefaultAsync(new AssignmentByIdSpec(request.AssignmentId));
        _ = assignment ?? throw new NotFoundException(_t["Assignment {0} Not  Found.", request.AssignmentId]);

        if (assignment.EndTime < DateTime.Now)
        {
            throw new ForbiddenException(_t["Can't update this submit."]);
        }

        var currentUserId = _currentUser.GetUserId();
        if (assignment.CreatedBy != currentUserId)
        {
            throw new ForbiddenException(_t["Can't update this submit."]);
        }

        assignment.UpdateStatusSubmitAssignment(request.Status);
        await _assignmentRepo.UpdateAsync(assignment);
        return assignment.Id;
    }
}