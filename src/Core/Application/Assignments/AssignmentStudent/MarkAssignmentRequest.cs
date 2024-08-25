using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.Assignments.AssignmentStudent;
public class MarkAssignmentRequest : IRequest<Guid>
{
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public float Score { get; set; }
    public string? Comment { get; set; }
}

public class MarkAssignmentRequestHandler : IRequestHandler<MarkAssignmentRequest, Guid>
{
    private readonly IRepository<Assignment> _assignmentRepo;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<Classes> _classesRepo;
    private readonly IStringLocalizer<MarkAssignmentRequestHandler> _t;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<GroupPermissionInClass> _groupPermissionRepo;
    private readonly IRepository<TeacherPermissionInClass> _teacherPermissionRepo;

    public MarkAssignmentRequestHandler(
        IRepository<Assignment> assignmentRepo,
        IRepository<Student> studentRepo,
        IStringLocalizer<MarkAssignmentRequestHandler> t,
        IRepository<Classes> classesRepo,
        ICurrentUser currentUser,
        IRepository<GroupPermissionInClass> groupPermissionRepo,
        IRepository<TeacherPermissionInClass> teacherPermissionRepo)
    {
        _assignmentRepo = assignmentRepo;
        _studentRepo = studentRepo;
        _t = t;
        _classesRepo = classesRepo;
        _currentUser = currentUser;
        _groupPermissionRepo = groupPermissionRepo;
        _teacherPermissionRepo = teacherPermissionRepo;
    }

    public async Task<Guid> Handle(MarkAssignmentRequest request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepo.FirstOrDefaultAsync(new AssignmentByIdSpec(request.AssignmentId));
        _ = assignment ?? throw new NotFoundException(_t["Assignment {0} Not  Found.", request.AssignmentId]);

        var userId = _currentUser.GetUserId();

        foreach (var class1 in assignment.AssignmentClasses)
        {
            var classroom = await _classesRepo.FirstOrDefaultAsync(new ClassByIdSpec(class1.ClassesId, userId))
                ?? throw new NotFoundException(_t["Class not found", class1.ClassesId]);

            if (classroom.CreatedBy != userId && assignment.CreatedBy != userId)
            {
                var groupPermissionSpec = new GroupPermissionClassByUserIdAndClassIdSpec(userId, class1.ClassesId);
                var teacherPermissionSpec = new TeacherPermissionCLassByUserIdAndClassIdSpec(userId, class1.ClassesId);

                var listPermission = new List<PermissionInClassDto>();

                listPermission.AddRange(await _groupPermissionRepo.ListAsync(groupPermissionSpec));
                listPermission.AddRange((await _teacherPermissionRepo
                                                .ListAsync(teacherPermissionSpec))
                                                .Where(x => !listPermission.Any(lp => lp.PermissionType == x.PermissionType)));

                if (!listPermission.Any(x => x.PermissionType == PermissionType.AssignAssignment || x.PermissionType == PermissionType.Marking))
                    throw new ForbiddenException(_t["You don't have mark permission"]);
            }
        }

        assignment.MarkScoreForSubmission(request.StudentId, request.Score, request.Comment);
        await _assignmentRepo.UpdateAsync(assignment);

        return assignment.Id;
    }
}
