using FSH.WebApi.Domain.Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Assignments.AssignmentStudent;
public class SubmitAssignmentRequest : IRequest
{
    public Guid AssignmentId { get; set; }
    public string? AnswerRaw { get; set; }
    public string? AttachmentPath { get;  set; }
}

public class SubmitAssignmentRequestValidator : CustomValidator<SubmitAssignmentRequest>
{
    public SubmitAssignmentRequestValidator(
        IRepository<Assignment> assignmentRepo,
        IStringLocalizer<SubmitAssignmentRequestValidator> T)
    {
        RuleFor(x => x.AssignmentId)
            .MustAsync(async (assignmentId, ct) => await assignmentRepo.GetByIdAsync(assignmentId, ct) is not null)
                .WithMessage((_, assignmentId) => T["Assignment {0} Not Found", assignmentId]);
    }
}

public class SubmitAssignmentRequestHandler : IRequestHandler<SubmitAssignmentRequest>
{
    private readonly IRepository<Assignment> _assignmentRepo;
    private readonly IStringLocalizer _t;
    private readonly ISerializerService _serializerService;
    private readonly ICurrentUser _currentUser;
    public SubmitAssignmentRequestHandler(
        IRepository<Assignment> assignmentRepo,
        IStringLocalizer<SubmitAssignmentRequestHandler> t,
        ISerializerService serializerService,
        ICurrentUser currentUser)
    {
        _assignmentRepo = assignmentRepo;
        _t = t;
        _serializerService = serializerService;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(SubmitAssignmentRequest request, CancellationToken cancellationToken)
    {

        var assignment = await _assignmentRepo.FirstOrDefaultAsync(new AssignmentByIdSpec(request.AssignmentId));
        if (assignment is null)
            throw new NotFoundException(_t["Assignment {0} Not Found", request.AssignmentId]);

        var currentUserId = _currentUser.GetUserId();
        assignment.SubmitAssignment(currentUserId, request.AnswerRaw, request.AttachmentPath);

        await _assignmentRepo.UpdateAsync(assignment);

        return Unit.Value;
    }
}
                