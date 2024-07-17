using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
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
                    classdto.Students = null;
                }

                if (!listPermission.Any(x => x.PermissionType == PermissionType.AssignAssignment
                                            && x.PermissionType == PermissionType.Marking))
                {
                    classdto.Assignments = null;
                    classdto.Papers = null;
                }

                classdto.Permissions = listPermission;
            }
            else
            {
                //classdto.Assignments = null;
                //classdto.Papers = null;
                classdto.Students = null;
            }
        }

        return classdto;
    }
}
