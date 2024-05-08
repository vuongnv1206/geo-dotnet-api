using FSH.WebApi.Application.TeacherGroup.PermissionClasses.Specs;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.PermissionClasses;
public class BulkUpdateGroupPermissionInClassRequest : IRequest<Guid>
{
    public Guid GroupTeacherId { get; set; }
    public List<PermissionInClassDto> PermissionInClassDtos { get; set; }
}


public class BulkUpdateGroupPermissionInClassRequestHandler : IRequestHandler<BulkUpdateGroupPermissionInClassRequest, Guid>
{
    private readonly IRepository<GroupTeacher> _repository;
    private readonly IRepository<Classes> _repositoryClass;
    private readonly IRepository<GroupPermissionInClass> _repositoryPermission;
    private readonly IStringLocalizer _t;

    public BulkUpdateGroupPermissionInClassRequestHandler(IRepository<GroupTeacher> repository,IRepository<Classes> repositoryClass, IStringLocalizer<BulkUpdateGroupPermissionInClassRequestHandler> t, IRepository<GroupPermissionInClass> repositoryPermission)
    {
        _repository = repository;
        _repositoryClass = repositoryClass;
        _t = t;
        _repositoryPermission = repositoryPermission;
    }

    public async Task<DefaultIdType> Handle(BulkUpdateGroupPermissionInClassRequest request, CancellationToken cancellationToken)
    {
        var groupTeacher = await _repository.FirstOrDefaultAsync(new GroupTeacherByIdWithPermissionSpec(request.GroupTeacherId),cancellationToken);
        _ = groupTeacher ?? throw new NotFoundException(_t["GroupTeacher {0} Not Found."]);

       
        await _repositoryPermission.DeleteRangeAsync(groupTeacher.GroupPermissionInClasses);

        foreach (var item in request.PermissionInClassDtos)
        {
            var classroom = _repositoryClass.GetByIdAsync(item.ClassId,cancellationToken);
            if (classroom is null)
            {
                throw new NotFoundException(_t["Classroom{0} Not Found.", item.ClassId]);
            }

            await _repositoryPermission.AddAsync(new GroupPermissionInClass
            {
                ClassId = item.ClassId,
                GroupTeacherId = request.GroupTeacherId,
                PermissionType = item.PermissionType,
            });

        }
        return default(DefaultIdType);
    }
}
