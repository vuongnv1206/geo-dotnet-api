using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Assignment;
using Mapster;

namespace FSH.WebApi.Application.Assignments.AssignmentStudent;
public class GetSubmissionAssignmentRequest : IRequest<List<SubmissionAssignmentDto>>
{
    public Guid AssignmentId { get; set; }
    public GetSubmissionAssignmentRequest(Guid assignmentId)
    {
        AssignmentId = assignmentId;
    }
}

public class GetSubmissionAssignmentRequestHandler : IRequestHandler<GetSubmissionAssignmentRequest, List<SubmissionAssignmentDto>>
{
    private readonly IRepository<Assignment> _repositoryAssignment;
    private readonly IStringLocalizer<GetSubmissionAssignmentRequestHandler> _t;
    private readonly IUserService _userService;

    public GetSubmissionAssignmentRequestHandler(IRepository<Assignment> repositoryAssignment, IStringLocalizer<GetSubmissionAssignmentRequestHandler> t, IUserService userService)
    {
        _repositoryAssignment = repositoryAssignment;
        _t = t;
        _userService = userService;
    }

    public async Task<List<SubmissionAssignmentDto>> Handle(GetSubmissionAssignmentRequest request, CancellationToken cancellationToken)
    {
        var assignment = await _repositoryAssignment.FirstOrDefaultAsync(new AssignmentByIdSpec(request.AssignmentId));
        _ = assignment ?? throw new NotFoundException(_t["Assignment {0} Not  Found.", request.AssignmentId]);

        var submissionAssignmentDto = assignment.AssignmentStudents.Adapt<List<SubmissionAssignmentDto>>();

        return submissionAssignmentDto;
    }
}
