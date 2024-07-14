using FSH.WebApi.Application.Assignments.AssignmentClasses;
using FSH.WebApi.Application.Assignments.AssignmentStudent;
using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
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
    public List<Guid>? StudentIds { get; set; }

}

public class UpdateAssignmentRequestHandler : IRequestHandler<UpdateAssignmentRequest, Guid>
{
    private readonly IRepository<Assignment> _repository;
    private readonly IStringLocalizer<UpdateAssignmentRequestHandler> _t;
    private readonly IFileStorageService _file;
    private readonly IRepository<Classes> _classRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<GroupPermissionInClass> _groupPermissionRepo;
    private readonly IRepository<TeacherPermissionInClass> _teacherPermissionRepo;
    private readonly IMediator _mediator;

    public UpdateAssignmentRequestHandler(
        IRepository<Assignment> repository,
        IStringLocalizer<UpdateAssignmentRequestHandler> t,
        IFileStorageService file,
        IRepository<Classes> classRepository,
        ICurrentUser currentUser,
        IRepository<GroupPermissionInClass> groupPermissionRepo,
        IRepository<TeacherPermissionInClass> teacherPermissionRepo,
        IMediator mediator)
    {
        _repository = repository;
        _t = t;
        _file = file;
        _classRepository = classRepository;
        _currentUser = currentUser;
        _groupPermissionRepo = groupPermissionRepo;
        _teacherPermissionRepo = teacherPermissionRepo;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(UpdateAssignmentRequest request, CancellationToken cancellationToken)
    {
        var assignment = await _repository.FirstOrDefaultAsync(new AssignmentByIdSpec(request.Id), cancellationToken)
            ?? throw new NotFoundException(_t["Assignment {0} Not Found.", request.Id]);

        var userId = _currentUser.GetUserId();
        foreach (var classId in request.ClassIds)
        {
            var classroom = await _classRepository.FirstOrDefaultAsync(new ClassByIdSpec(classId, userId))
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
            // Lấy danh sách các lớp hiện tại được giao bài tập
            var currentClassIds = updatedAssignment.AssignmentClasses.Select(ac => ac.ClassesId).ToList();
            // Lấy danh sách các lớp cần thêm mới
            var newClassIds = request.ClassIds.Except(currentClassIds).ToList();
            // Lấy danh sách các lớp cần xóa
            var removedClassIds = currentClassIds.Except(request.ClassIds).ToList();
            // Thêm các lớp mới vào AssignmentClasses
            if (newClassIds.Any())
            {
                var assignRequest = new AssignAssignmentToClassRequest(request.Id, newClassIds);
                await _mediator.Send(assignRequest, cancellationToken);
            }
            // Xóa các lớp không còn trong danh sách
            if (removedClassIds.Any())
            {
                foreach (var classId in removedClassIds)
                {
                    var removeRequest = new RemoveAssignmentFromClassRequest(request.Id, classId);
                    await _mediator.Send(removeRequest, cancellationToken);
                }
            }

        }
        else
        {
            // Nếu ClassIds là null, xóa tất cả các lớp hiện tại
            var currentClassIds = updatedAssignment.AssignmentClasses.Select(ac => ac.ClassesId).ToList();
            foreach (var classId in currentClassIds)
            {
                var removeRequest = new RemoveAssignmentFromClassRequest(request.Id, classId);
                await _mediator.Send(removeRequest, cancellationToken);
            }
        }

        if (request.StudentIds != null)
        {
            // Lấy danh sách các học sinh hiện tại được giao bài tập
            var currentStudentIds = updatedAssignment.AssignmentStudents.Select(a => a.StudentId).ToList();
            // Lấy danh sách các học sinh cần thêm mới
            var newStudentIds = request.StudentIds.Except(currentStudentIds).ToList();
            // Lấy danh sách các học sinh cần xóa
            var removedStudentIds = currentStudentIds.Except(request.StudentIds).ToList();

            // Thêm các học sinh mới vào AssignmentStudents
            if (newStudentIds.Any())
            {
                var assignRequest = new AssignAssignmentToStudentsRequest
                {
                    AssignmentId = request.Id,
                    StudentIds = newStudentIds
                };
                await _mediator.Send(assignRequest, cancellationToken);
            }

            // Xóa các học sinh không còn trong danh sách
            if (removedStudentIds.Any())
            {
                foreach (var studentId in removedStudentIds)
                {
                    var removeRequest = new RemoveAssignmentOfStudentRequest
                    {
                        AssignmentId = request.Id,
                        StudentId = studentId
                    };
                    await _mediator.Send(removeRequest, cancellationToken);
                }
            }
        }
        else
        {
            // Nếu StudentIds là null, xóa tất cả các học sinh hiện tại
            var currentStudentIds = updatedAssignment.AssignmentStudents.Select(a => a.StudentId).ToList();
            foreach (var studentId in currentStudentIds)
            {
                var removeRequest = new RemoveAssignmentOfStudentRequest
                {
                    AssignmentId = request.Id,
                    StudentId = studentId
                };
                await _mediator.Send(removeRequest, cancellationToken);
            }
        }

        await _repository.UpdateAsync(updatedAssignment, cancellationToken);

        return request.Id;
    }
}