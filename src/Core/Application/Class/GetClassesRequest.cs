using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;

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

    public GetClassRequestHandler(
        IRepository<Classes> repository,
        IStringLocalizer<GetClassRequestHandler> t,
        ICurrentUser currentUser,
        IRepository<TeacherPermissionInClass> teacherPermissionRepo,
        IRepository<GroupPermissionInClass> groupPermissionRepo)
    {
        _repository = repository;
        _t = t;
        _currentUser = currentUser;
        _teacherPermissionRepo = teacherPermissionRepo;
        _groupPermissionRepo = groupPermissionRepo;
    }

    public async Task<ClassDto> Handle(GetClassesRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        var classroom = await _repository.FirstOrDefaultAsync(new ClassByIdWithGroupClassSpec(request.Id), cancellationToken)
             ?? throw new NotFoundException(_t["Classes {0} Not Found.", request.Id]);
        if (userId != classroom.OwnerId)
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
                classroom.Students = null;
            }

            if (!listPermission.Any(x => x.PermissionType == PermissionType.AssignAssignment
                                        && x.PermissionType == PermissionType.Marking))
            {
                classroom.Assignments = null;
            }

            classroom.Permissions = listPermission;
        }

        return classroom;
    }
}
