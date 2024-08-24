using FSH.WebApi.Application.Assignments.AssignmentClasses;
using FSH.WebApi.Application.Assignments.AssignmentStudent;
using FSH.WebApi.Application.Class;
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
    public List<Guid>? classesId { get; set; }
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

        // Kiểm tra quyền cập nhật Assignment
        foreach (var class1 in assignment.AssignmentClasses)
        {
            var classroom = await _classRepository.FirstOrDefaultAsync(new ClassByIdSpec(class1.ClassesId, userId))
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

                if (!listPermission.Any(x => x.PermissionType == PermissionType.AssignAssignment))
                    throw new NotFoundException(_t["Classes {0} Not Found.", class1.ClassesId]);
            }
        }

        //Check nếu gia hạn thời gian nộp bài assignment thì xóa hết điểm và comment của assignmentStudent ,đồng thời update tất cả status thành Doing
        if (request.EndTime != null && request.EndTime > assignment.EndTime)
        {
            assignment.ExtendTimeAssignment();
        }


        // Cập nhật thông tin Assignment
        var updatedAssignment = assignment.Update(
            request.Name,
            request.StartTime,
            request.EndTime,
            request.Attachment,
            request.Content,
            request.CanViewResult,
            request.RequireLoginToSubmit,
            request.SubjectId);

        // Cập nhật các lớp học liên quan
        if (request.classesId != null)
        {
            var currentClassIds = updatedAssignment.AssignmentClasses.Select(ac => ac.ClassesId).ToList();
            var newClassIds = request.classesId.Except(currentClassIds).ToList();
            var removedClassIds = currentClassIds.Except(request.classesId).ToList();

            if (newClassIds.Any())
            {
                var assignRequest = new AssignAssignmentToClassRequest(request.Id, newClassIds);
                await _mediator.Send(assignRequest, cancellationToken);
            }

            if (removedClassIds.Any())
            {
                foreach (var classId in removedClassIds)
                {
                    var removeRequest = new RemoveAssignmentFromClassRequest(request.Id, classId);
                    await _mediator.Send(removeRequest, cancellationToken);
                }
            }
        }

        // Cập nhật các học sinh liên quan
        if (request.StudentIds != null)
        {
            var currentStudentIds = updatedAssignment.AssignmentStudents.Select(a => a.StudentId).ToList();
            var newStudentIds = request.StudentIds.Except(currentStudentIds).ToList();
            var removedStudentIds = currentStudentIds.Except(request.StudentIds).ToList();

            if (newStudentIds.Any())
            {
                var assignRequest = new AssignAssignmentToStudentsRequest
                {
                    AssignmentId = request.Id,
                    StudentIds = newStudentIds
                };
                await _mediator.Send(assignRequest, cancellationToken);
            }

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

        

        await _repository.UpdateAsync(updatedAssignment, cancellationToken);


      
       

      


        return request.Id;
    }
}
