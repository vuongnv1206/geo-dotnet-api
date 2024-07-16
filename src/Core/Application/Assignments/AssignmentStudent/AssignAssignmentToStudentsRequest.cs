using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Assignments.AssignmentStudent;
public class AssignAssignmentToStudentsRequest : IRequest<Guid>
{
    public Guid AssignmentId { get; set; }
    public required List<Guid> StudentIds { get; set; }
}

public class AssignAssignmentToStudentRequestValidator : CustomValidator<AssignAssignmentToStudentsRequest>
{
    public AssignAssignmentToStudentRequestValidator()
    {

    }
}

public class AssignAssignmentToStudentsRequestHandler : IRequestHandler<AssignAssignmentToStudentsRequest, Guid>
{
    private readonly IRepository<Student> _studentRepository;
    private readonly IRepository<Assignment> _assignmentRepository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;

    public AssignAssignmentToStudentsRequestHandler(
        IRepository<Student> studentRepository,
        IRepository<Assignment> assignmentRepository,
        IStringLocalizer<AssignAssignmentToStudentsRequestHandler> t,
        ICurrentUser currentUser)
        => (_studentRepository, _assignmentRepository, _t, _currentUser)
            = (studentRepository, assignmentRepository, t, currentUser);

    public async Task<Guid> Handle(AssignAssignmentToStudentsRequest request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.FirstOrDefaultAsync(new AssignmentByIdSpec(request.AssignmentId));
        _ = assignment ?? throw new NotFoundException(_t["Assignment {0} Not Found.", request.AssignmentId]);
        foreach (var studentId in request.StudentIds)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            _ = student ?? throw new NotFoundException(_t["Student {0} Not Found.", studentId]);
            assignment.AssignAssignmentToStudent(student.Id);
        }

        await _assignmentRepository.UpdateAsync(assignment);

        return assignment.Id;
    }
}