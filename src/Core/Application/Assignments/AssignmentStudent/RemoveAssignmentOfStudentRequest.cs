using FSH.WebApi.Application.Class;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Assignments.AssignmentStudent;
public class RemoveAssignmentOfStudentRequest : IRequest<Guid>
{
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
}

public class RemoveAssignmentOfStudentRequestValidator : CustomValidator<RemoveAssignmentOfStudentRequest>
{
    public RemoveAssignmentOfStudentRequestValidator()
    {

    }

}

public class RemoveAssignmentOfStudentRequestHandler : IRequestHandler<RemoveAssignmentOfStudentRequest, Guid>
{
    private readonly IRepository<Student> _studentRepository;
    private readonly IRepository<Assignment> _assignmentRepository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;

    public RemoveAssignmentOfStudentRequestHandler(
        IRepository<Student> studentRepository,
        IRepository<Assignment> assignmentRepository,
        IStringLocalizer<RemoveAssignmentOfStudentRequestHandler> t,
        ICurrentUser currentUser)
        => (_studentRepository, _assignmentRepository, _t, _currentUser)
            = (studentRepository, assignmentRepository, t, currentUser);

    public async Task<Guid> Handle(RemoveAssignmentOfStudentRequest request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.FirstOrDefaultAsync(new AssignmentByIdSpec(request.AssignmentId));
        _ = assignment ?? throw new NotFoundException(_t["Assignment {0} Not Found", request.AssignmentId]);

        assignment.RemoveAssignmentOfStudent(request.StudentId);

        await _assignmentRepository.UpdateAsync(assignment);


        return assignment.Id;
    }
}