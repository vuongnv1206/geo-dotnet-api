using FSH.WebApi.Application.Class.Specs;
using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Common.Events;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.Assignments;

public class DeleteAssignmentRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }

    public DeleteAssignmentRequest(DefaultIdType id) => Id = id;
}

public class DeleteAssignmentRequestHandler : IRequestHandler<DeleteAssignmentRequest, DefaultIdType>
{
    private readonly IRepository<Assignment> _repository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<Classes> _classRepo;
    private readonly IRepository<GroupPermissionInClass> _groupPermissionRepo;
    private readonly IRepository<TeacherPermissionInClass> _teacherPermissionRepo;

    public DeleteAssignmentRequestHandler(
        IRepository<Assignment> repository,
        IStringLocalizer<DeleteAssignmentRequestHandler> t,
        ICurrentUser currentUser,
        IRepository<Classes> classRepo,
        IRepository<GroupPermissionInClass> groupPermissionRepo,
        IRepository<TeacherPermissionInClass> teacherPermissionRepo)
    {
        _repository = repository;
        _t = t;
        _currentUser = currentUser;
        _classRepo = classRepo;
        _groupPermissionRepo = groupPermissionRepo;
        _teacherPermissionRepo = teacherPermissionRepo;
    }

    public async Task<DefaultIdType> Handle(DeleteAssignmentRequest request, CancellationToken cancellationToken)
    {
        var assignment = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(_t["Assignment {0} Not Found."]);

        var userId = _currentUser.GetUserId();

        var classOfStudent = await _classRepo.FirstOrDefaultAsync(new ClassByStudentClassIdSpec(request.Id));
        if (classOfStudent.CreatedBy != userId && assignment.CreatedBy != userId)
        {
            var groupPermissionSpec = new GroupPermissionClassByUserIdAndClassIdSpec(userId, classOfStudent.Id);
            var teacherPermissionSpec = new TeacherPermissionCLassByUserIdAndClassIdSpec(userId, classOfStudent.Id);

            var listPermission = new List<PermissionInClassDto>();

            listPermission.AddRange(await _groupPermissionRepo.ListAsync(groupPermissionSpec));
            listPermission.AddRange((await _teacherPermissionRepo
                                            .ListAsync(teacherPermissionSpec))
                                            .Where(x => !listPermission.Any(lp => lp.PermissionType == x.PermissionType)));

            if (!listPermission.Any(x => x.PermissionType == PermissionType.AssignAssignment))
                throw new NotFoundException(_t["Assignment {0} Not Found.", request.Id]);
        }

        // Add Domain Events to be raised after the commit
        assignment.DomainEvents.Add(EntityDeletedEvent.WithEntity(assignment));

        await _repository.DeleteAsync(assignment, cancellationToken);

        return request.Id;
    }
}