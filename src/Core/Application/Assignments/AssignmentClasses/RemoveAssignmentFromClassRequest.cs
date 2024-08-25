using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.Assignments.AssignmentClasses;

public class RemoveAssignmentFromClassRequest : IRequest<Guid>
{
    public Guid AssignmentId { get; set; }
    public Guid ClassId { get; set; }

    public RemoveAssignmentFromClassRequest(DefaultIdType assignmentId, DefaultIdType classId)
    {
        AssignmentId = assignmentId;
        ClassId = classId;
    }
}

public class RemoveAssignmentFromClassRequestValidator : CustomValidator<RemoveAssignmentFromClassRequest>
{
    public RemoveAssignmentFromClassRequestValidator()
    {

    }
}

public class RemoveAssignmentFromClassRequestHandler : IRequestHandler<RemoveAssignmentFromClassRequest, Guid>
{
    private readonly IRepository<Classes> _classesRepository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<Assignment> _assignmentRepo;
    private readonly IRepository<GroupPermissionInClass> _groupPermissionRepo;
    private readonly IRepository<TeacherPermissionInClass> _teacherPermissionRepo;

    public RemoveAssignmentFromClassRequestHandler(
        IStringLocalizer<RemoveAssignmentFromClassRequestHandler> t,
        IRepository<Classes> classesRepository,
        ICurrentUser currentUser,
        IRepository<Assignment> assignmentRepo,
        IRepository<GroupPermissionInClass> groupPermissionRepo,
        IRepository<TeacherPermissionInClass> teacherPermissionRepo)
    {
        _t = t;
        _classesRepository = classesRepository;
        _currentUser = currentUser;
        _assignmentRepo = assignmentRepo;
        _groupPermissionRepo = groupPermissionRepo;
        _teacherPermissionRepo = teacherPermissionRepo;
    }

    public async Task<DefaultIdType> Handle(RemoveAssignmentFromClassRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        var assignment = await _assignmentRepo.FirstOrDefaultAsync(new AssignmentByIdSpec(request.AssignmentId));
        _ = assignment ?? throw new NotFoundException(_t["Assignment {0} Not Found", request.AssignmentId]);

        var classroom = await _classesRepository.FirstOrDefaultAsync(new ClassesByIdSpec(request.ClassId));
        _ = classroom ?? throw new NotFoundException(_t["Class {0} Not Found.", request.ClassId]);

        if (classroom.CreatedBy != userId && assignment.CreatedBy != userId)
        {
            var groupPermissionSpec = new GroupPermissionClassByUserIdAndClassIdSpec(userId, classroom.Id);
            var teacherPermissionSpec = new TeacherPermissionCLassByUserIdAndClassIdSpec(userId, classroom.Id);

            var listPermission = new List<PermissionInClassDto>();

            listPermission.AddRange(await _groupPermissionRepo.ListAsync(groupPermissionSpec));
            listPermission.AddRange((await _teacherPermissionRepo
                                            .ListAsync(teacherPermissionSpec))
                                            .Where(x => !listPermission.Any(lp => lp.PermissionType == x.PermissionType)));

            if (!listPermission.Any(x => x.PermissionType == PermissionType.AssignAssignment))
                throw new NotFoundException(_t["You don't have assignment permission"]);
        }

        var studentIds = classroom.UserClasses.Select(x => x.StudentId).ToList();
        assignment.RemoveSubmissionFromClass(studentIds);

        classroom.RemoveAssignment(request.AssignmentId);
        await _classesRepository.UpdateAsync(classroom, cancellationToken);

        return classroom.Id;
    }
}