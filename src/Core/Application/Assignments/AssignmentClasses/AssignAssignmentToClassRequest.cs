using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Assignments.AssignmentClasses;

public class AssignAssignmentToClassRequest : IRequest<Guid>
{
    public Guid AssignmentId { get; set; }
    public List<Guid> ClassIds { get; set; }

    public AssignAssignmentToClassRequest(DefaultIdType assignmentId, List<DefaultIdType> classIds)
    {
        AssignmentId = assignmentId;
        ClassIds = classIds;
    }
}

public class AssignAssignmentToClassRequestValidator : CustomValidator<AssignAssignmentToClassRequest>
{
    public AssignAssignmentToClassRequestValidator()
    {

    }
}

public class AssignAssignmentToClassRequestHandler : IRequestHandler<AssignAssignmentToClassRequest, Guid>
{
    private readonly IRepository<Classes> _classesRepository;
    private readonly IUserService _userService;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<Assignment> _assignmentRepository;

    public AssignAssignmentToClassRequestHandler(
        IRepository<Classes> classesRepository,
        IUserService userService,
        IStringLocalizer<AssignAssignmentToClassRequestHandler> t,
        ICurrentUser currentUser,
        IRepository<Assignment> assignmentRepository)
        => (_classesRepository, _userService, _t, _currentUser, _assignmentRepository)
            = (classesRepository, userService, t, currentUser, assignmentRepository);

    public async Task<Guid> Handle(AssignAssignmentToClassRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var assignment = await _assignmentRepository.FirstOrDefaultAsync(new AssignmentByIdSpec(request.AssignmentId));
        _ = assignment ?? throw new NotFoundException(_t["Assignment {0} Not Found.", request.AssignmentId]);

        foreach (var classId in request.ClassIds)
        {
            var classroom = await _classesRepository.FirstOrDefaultAsync(new ClassByIdSpec(classId, userId));
            _ = classroom ?? throw new NotFoundException(_t["Class {0} Not Found.", classId]);
            assignment.AssignAssignmentToClass(classroom.Id);

            var studentIds = classroom.UserClasses.Select(x => x.StudentId).ToList();
            assignment.AssignAssignmentToStudents(studentIds);
        }

        await _assignmentRepository.UpdateAsync(assignment);

        return assignment.Id;
    }
}
