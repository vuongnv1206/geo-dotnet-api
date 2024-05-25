using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.PermissionClasses;
public class BulkUpdateTeacherPermissionInClassRequest : IRequest<Guid>
{
    public Guid TeacherId { get; set; }
    public List<PermissionInClassDto>? PermissionInClassDtos { get; set; }
}

public class BulkUpdateTeacherPermissionInClassRequestHandler : IRequestHandler<BulkUpdateTeacherPermissionInClassRequest, Guid>
{
    private readonly IRepositoryWithEvents<TeacherPermissionInClass> _teacherPermissionRepo;
    private readonly IRepositoryWithEvents<TeacherTeam> _teacherTeamRepo;
    private readonly IReadRepository<Classes> _classRepo;
    private readonly IStringLocalizer _t;

    public BulkUpdateTeacherPermissionInClassRequestHandler(
        IRepositoryWithEvents<TeacherPermissionInClass> teacherPermissionRepo,
        IRepositoryWithEvents<TeacherTeam> teacherTeamRepo,
        IReadRepository<Classes> classRepo,
        IStringLocalizer<BulkUpdateTeacherPermissionInClassRequestHandler> t)
    {
        _teacherPermissionRepo = teacherPermissionRepo;
        _teacherTeamRepo = teacherTeamRepo;
        _classRepo = classRepo;
        _t = t;
    }

    public async Task<DefaultIdType> Handle(BulkUpdateTeacherPermissionInClassRequest request, CancellationToken cancellationToken)
    {
        var teacherTeam = await _teacherTeamRepo.FirstOrDefaultAsync(
            new TeacherTeamByIdWithPermissionSpec(request.TeacherId), cancellationToken);
        if (teacherTeam is null)
            throw new NotFoundException(_t["Teacher Not Found."]);

        foreach (var teacher in teacherTeam.TeacherPermissionInClasses)
        {
            await _teacherPermissionRepo.DeleteAsync(teacher);
        }

        foreach (var permissionInClass in request.PermissionInClassDtos)
        {
            var classRoom = _classRepo.GetByIdAsync(permissionInClass.ClassId, cancellationToken);
            if (classRoom.Result is null) continue;

            await _teacherPermissionRepo.AddAsync(new TeacherPermissionInClass
            {
                TeacherTeamId = request.TeacherId,
                ClassId = permissionInClass.ClassId,
                PermissionType = permissionInClass.PermissionType
            });
        }

        return default(DefaultIdType);
    }
}
