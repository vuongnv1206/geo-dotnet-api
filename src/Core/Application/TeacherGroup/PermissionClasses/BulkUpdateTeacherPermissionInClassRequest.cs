using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace FSH.WebApi.Application.TeacherGroup.PermissionClasses;
public class BulkUpdateTeacherPermissionInClassRequest : IRequest<Guid>
{
    public Guid TeacherId { get; set; }
    public List<PermissionInClassDto> PermissionInClassDtos { get; set; }
}

public class PermissionInClassDto
{
    public Guid ClassId { get; set; }
    public PermissionType PermissionType { get; set; }
}

public class TeacherTeamByIdWithPermissionSpec : Specification<TeacherTeam>, ISingleResultSpecification
{
    public TeacherTeamByIdWithPermissionSpec(Guid id)
    {
        Query.Where(x => x.Id == id).Include(x => x.TeacherPermissionInClasses);
    }
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
        var teacherTeam = await _teacherTeamRepo.FirstOrDefaultAsync
            (new TeacherTeamByIdWithPermissionSpec(request.TeacherId), cancellationToken);
        if (teacherTeam is null)
            throw new NotFoundException(_t["Teacher Not Found."]);

        foreach(var teacher in teacherTeam.TeacherPermissionInClasses)
        {
            await _teacherPermissionRepo.DeleteAsync(teacher);
        }

        foreach(var permissionInClass in request.PermissionInClassDtos)
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
