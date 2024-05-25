using FSH.WebApi.Application.TeacherGroup.PermissionClasses.Specs;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.PermissionClasses;
public class BulkUpdateGroupPermissionInClassRequest : IRequest<Guid>
{
    public Guid GroupTeacherId { get; set; }
    public List<PermissionInClassDto> PermissionInClassDtos { get; set; } = new();
}

public class BulkUpdateGroupPermissionInClassRequestHandler : IRequestHandler<BulkUpdateGroupPermissionInClassRequest, Guid>
{
    private readonly IRepository<GroupTeacher> _repository;
    private readonly IRepository<Classes> _repositoryClass;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;

    public BulkUpdateGroupPermissionInClassRequestHandler(
        IRepository<GroupTeacher> repository,
        IRepository<Classes> repositoryClass,
        IStringLocalizer<BulkUpdateGroupPermissionInClassRequestHandler> t,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _repositoryClass = repositoryClass;
        _t = t;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType> Handle(BulkUpdateGroupPermissionInClassRequest request, CancellationToken cancellationToken)
    {
        var groupTeacher = await _repository.FirstOrDefaultAsync(
            new GroupTeacherByIdWithPermissionSpec(request.GroupTeacherId), cancellationToken)
        ?? throw new NotFoundException(_t["GroupTeacher {0} Not Found."]);

        if (groupTeacher.CanUpdate(_currentUser.GetUserId()))
        {
            throw new ForbiddenException(_t["You don't have this permission."]);
        }

        var existingPermissions = groupTeacher.GroupPermissionInClasses.ToList();

        foreach (var existPermission in existingPermissions)
        {
            if (!request.PermissionInClassDtos
                .Any(x => x.ClassId == existPermission.ClassId
                    && x.PermissionType == existPermission.PermissionType))
            {
                groupTeacher.RemovePermission(existPermission);
            }
        }

        foreach (var item in request.PermissionInClassDtos)
        {
            if (!groupTeacher.GroupPermissionInClasses
                .Any(x => x.ClassId == item.ClassId
                    && x.PermissionType == item.PermissionType))
            {
                var classroom = _repositoryClass.GetByIdAsync(item.ClassId,cancellationToken);
                _ = classroom ?? throw new NotFoundException(_t["Classroom{0} Not Found.", item.ClassId]);

                groupTeacher.AddPermission(new GroupPermissionInClass
                {
                    ClassId = item.ClassId,
                    GroupTeacherId = request.GroupTeacherId,
                    PermissionType = item.PermissionType,
                });
            }
        }

        await _repository.UpdateAsync(groupTeacher);

        return groupTeacher.Id;
    }
}
