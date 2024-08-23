using FSH.WebApi.Application.Assignments.AssignmentClasses;
using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.UserStudents.Dto;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;
using Mapster;

namespace FSH.WebApi.Application.Class;
public class GetClassesRequest : IRequest<ClassDto>
{
    public Guid Id { get; set; }

    public GetClassesRequest(Guid id) => Id = id;
}

public class GetClassRequestHandler : IRequestHandler<GetClassesRequest, ClassDto>
{
    private readonly IRepository<Classes> _repository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<GroupPermissionInClass> _groupPermissionRepo;
    private readonly IRepository<TeacherPermissionInClass> _teacherPermissionRepo;
    private readonly IMediator _mediator;
    private readonly IUserService _userService;

    public GetClassRequestHandler(
        IRepository<Classes> repository,
        IStringLocalizer<GetClassRequestHandler> t,
        ICurrentUser currentUser,
        IRepository<TeacherPermissionInClass> teacherPermissionRepo,
        IRepository<GroupPermissionInClass> groupPermissionRepo,
        IMediator mediator,
        IUserService userService)
    {
        _repository = repository;
        _t = t;
        _currentUser = currentUser;
        _teacherPermissionRepo = teacherPermissionRepo;
        _groupPermissionRepo = groupPermissionRepo;
        _mediator = mediator;
        _userService = userService;
    }

    public async Task<ClassDto> Handle(GetClassesRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        var classroom = await _repository.FirstOrDefaultAsync(new ClassByIdSpec(request.Id, userId), cancellationToken)
             ?? throw new NotFoundException(_t["Classes {0} Not Found.", request.Id]);

        var classdto = classroom.Adapt<ClassDto>();

        if (userId != classdto.OwnerId)
        {
            if (_currentUser.IsInRole("Teacher"))
            {

                var groupPermissionSpec = new GroupPermissionClassByUserIdAndClassIdSpec(userId, request.Id);
                var teacherPermissionSpec = new TeacherPermissionCLassByUserIdAndClassIdSpec(userId, request.Id);

                var listPermission = new List<PermissionInClassDto>();

                listPermission.AddRange(await _groupPermissionRepo.ListAsync(groupPermissionSpec));
                listPermission.AddRange((await _teacherPermissionRepo
                                                .ListAsync(teacherPermissionSpec))
                                                .Where(x => !listPermission.Any(lp => lp.PermissionType == x.PermissionType)));

                if (!listPermission.Any())
                    throw new NotFoundException(_t["Classes {0} Not Found.", request.Id]);

                if (!listPermission.Any(x => x.PermissionType == PermissionType.ManageStudentList))
                {
                    classdto.Students = classroom.UserClasses?.Select(x => x.Student).Adapt<List<UserStudentDto>>();
                }

                if (!listPermission.Any(x => x.PermissionType == PermissionType.AssignAssignment
                                            && x.PermissionType == PermissionType.Marking))
                {
                    classdto.Assignments = await _mediator.Send(new GetAssignmentInClassRequest(request.Id));
                    classdto.Papers = await _mediator.Send(new GetPapersInClassRequest(request.Id));
                }

                classdto.Permissions = listPermission;
            }
            else
            {
                classdto.Assignments = await _mediator.Send(new GetAssignmentInClassRequest(request.Id));
                classdto.Papers = await _mediator.Send(new GetPapersInClassRequest(request.Id));
                classdto.Students = classroom.UserClasses?.Select(x => x.Student).Adapt<List<UserStudentDto>>();

            }
        }
        else
        {
            classdto.Assignments = await _mediator.Send(new GetAssignmentInClassRequest(request.Id));
            classdto.Papers = await _mediator.Send(new GetPapersInClassRequest(request.Id));
            classdto.Students = classroom.UserClasses?.Select(x => x.Student).Adapt<List<UserStudentDto>>();
        }

        var user = await _userService.GetAsync(classdto.OwnerId.ToString(), cancellationToken);
        if (user != null)
        {
            classdto.Owner = user;
        }

        return classdto;
    }
}
