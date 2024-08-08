using FSH.WebApi.Application.Assignments.AssignmentClasses;
using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Class.UserStudents;
using FSH.WebApi.Application.Common.FileStorage;
using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Common.Events;
using FSH.WebApi.Domain.TeacherGroup;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace FSH.WebApi.Application.Assignments;
public class CreateAssignmentRequest : IRequest<Guid>
{
    public string Name { get; set; } = default!;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Content { get; set; }
    public bool CanViewResult { get; set; }
    public bool RequireLoginToSubmit { get; set; }
    public Guid SubjectId { get; set; }
    public string? Attachment { get; set; }
    public List<Guid>? ClassIds { get; set; }
}

public class CreateAssignmentRequestHandler : IRequestHandler<CreateAssignmentRequest, Guid>
{
    private readonly IRepository<Assignment> _repository;
    private readonly IFileStorageService _file;
    private readonly IRepository<Classes> _classRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<GroupPermissionInClass> _groupPermissionRepo;
    private readonly IRepository<TeacherPermissionInClass> _teacherPermissionRepo;
    private readonly IStringLocalizer _t;
    private readonly IMediator _mediator;

    public CreateAssignmentRequestHandler(
        IRepository<Assignment> repository,
        IFileStorageService file,
        IRepository<Classes> classRepository,
        ICurrentUser currentUser,
        IRepository<GroupPermissionInClass> groupPermissionRepo,
        IRepository<TeacherPermissionInClass> teacherPermissionRepo,
        IStringLocalizer<CreateAssignmentRequestHandler> t,
        IMediator mediator)
    {
        _repository = repository;
        _file = file;
        _classRepository = classRepository;
        _currentUser = currentUser;
        _groupPermissionRepo = groupPermissionRepo;
        _teacherPermissionRepo = teacherPermissionRepo;
        _t = t;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(CreateAssignmentRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        foreach (var classId in request.ClassIds)
        {
            var classroom = await _classRepository.FirstOrDefaultAsync(new ClassByIdSpec(classId, userId))
                ?? throw new NotFoundException(_t["Class not found", classId]);

            if (classroom.CreatedBy != userId)
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

        //string attachmentPath = JsonSerializer.Serialize(request.Attachment);

        var assignment = new Assignment(request.Name.Trim(), request.StartTime, request.EndTime, request.Attachment, request.Content, request.CanViewResult, request.RequireLoginToSubmit, request.SubjectId);
        await _repository.AddAsync(assignment, cancellationToken);

        if (request.ClassIds != null)
        {
            await _mediator.Send(new AssignAssignmentToClassRequest(assignment.Id, request.ClassIds));
        }

        return assignment.Id;
    }
}