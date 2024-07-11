using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.Assignments;
public class UpdateAssignmentRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Content { get; set; }
    public bool CanViewResult { get; set; }
    public bool RequireLoginToSubmit { get; set; }
    public DefaultIdType? SubjectId { get; set; }
    public bool DeleteCurrentAttachment { get; set; } = false;
    public string? Attachment { get; set; }
    public List<Guid>? ClassIds { get; set; }

}

public class UpdateAssignmentRequestHandler : IRequestHandler<UpdateAssignmentRequest, Guid>
{
    private readonly IRepository<Assignment> _repository;
    private readonly IStringLocalizer _t;
    private readonly IFileStorageService _file;
    private readonly IRepository<Classes> _classRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<GroupPermissionInClass> _groupPermissionRepo;
    private readonly IRepository<TeacherPermissionInClass> _teacherPermissionRepo;

    public UpdateAssignmentRequestHandler(
        IRepository<Assignment> repository,
        IStringLocalizer<UpdateAssignmentRequestHandler> t,
        IFileStorageService file,
        IRepository<Classes> classRepository,
        ICurrentUser currentUser,
        IRepository<GroupPermissionInClass> groupPermissionRepo,
        IRepository<TeacherPermissionInClass> teacherPermissionRepo)
    {
        _repository = repository;
        _t = t;
        _file = file;
        _classRepository = classRepository;
        _currentUser = currentUser;
        _groupPermissionRepo = groupPermissionRepo;
        _teacherPermissionRepo = teacherPermissionRepo;
    }

    public async Task<Guid> Handle(UpdateAssignmentRequest request, CancellationToken cancellationToken)
    {
        var assignment = await _repository.FirstOrDefaultAsync(new AssignmentByIdSpec(request.Id), cancellationToken)
            ?? throw new NotFoundException(_t["Assignment {0} Not Found.", request.Id]);

        var userId = _currentUser.GetUserId();
        foreach (var classId in request.ClassIds)
        {
            var classroom = await _classRepository.FirstOrDefaultAsync(new ClassByIdSpec(classId))
                ?? throw new NotFoundException(_t["Class not found", classId]);

            if (classroom.CreatedBy != userId && assignment.CreatedBy != userId)
            {
                var groupPermissionSpec = new GroupPermissionClassByUserIdAndClassIdSpec(userId, classId);
                var teacherPermissionSpec = new TeacherPermissionCLassByUserIdAndClassIdSpec(userId, classId);

                var listPermission = new List<PermissionInClassDto>();

                listPermission.AddRange(await _groupPermissionRepo.ListAsync(groupPermissionSpec));
                listPermission.AddRange((await _teacherPermissionRepo
                                                .ListAsync(teacherPermissionSpec))
                                                .Where(x => !listPermission.Any(lp => lp.PermissionType == x.PermissionType)));

                if (!listPermission.Any(x => x.PermissionType == PermissionType.AssignAssignment))
                    throw new NotFoundException(_t["Classes {0} Not Found.", classId]);
            }
        }

        var updatedAssignment = assignment.Update(
            request.Name,
            request.StartTime,
            request.EndTime,
            request.Attachment,
            request.Content,
            request.CanViewResult,
            request.RequireLoginToSubmit,
            request.SubjectId);

        if (request.ClassIds != null)
        {
            updatedAssignment.AssignmentClasses.Clear();
            updatedAssignment.UpdateAssignmentFromClass(request.ClassIds.Select(x => new AssignmentClass(updatedAssignment.Id, x)).ToList());

        }

        await _repository.UpdateAsync(updatedAssignment, cancellationToken);

        return request.Id;
    }
}