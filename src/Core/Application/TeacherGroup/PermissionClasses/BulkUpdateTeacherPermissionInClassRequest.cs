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
    private readonly IRepositoryWithEvents<TeacherTeam> _teacherTeamRepo;
    private readonly IReadRepository<Classes> _classRepo;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;

    public BulkUpdateTeacherPermissionInClassRequestHandler(
        IRepositoryWithEvents<TeacherTeam> teacherTeamRepo,
        IReadRepository<Classes> classRepo,
        IStringLocalizer<BulkUpdateTeacherPermissionInClassRequestHandler> t,
        ICurrentUser currentUser)
    {
        _teacherTeamRepo = teacherTeamRepo;
        _classRepo = classRepo;
        _t = t;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType> Handle(BulkUpdateTeacherPermissionInClassRequest request, CancellationToken cancellationToken)
    {
        var teacherTeam = await _teacherTeamRepo.FirstOrDefaultAsync(
            new TeacherTeamByIdWithPermissionSpec(request.TeacherId), cancellationToken)
        ?? throw new NotFoundException(_t["Teacher Not Found."]);

        if (!teacherTeam.CanUpdate(_currentUser.GetUserId()))
        {
            throw new ForbiddenException(_t["You don't have this permission."]);
        }

        var existingPermissions = teacherTeam.TeacherPermissionInClasses?.ToList();

        foreach(var existPermission in existingPermissions)
        {
            if (!request.PermissionInClassDtos
                .Any(x => x.ClassId == existPermission.ClassId
                    && x.PermissionType == existPermission.PermissionType))
            {
                teacherTeam.RemovePermission(existPermission);
            }
        }

        foreach (var item in request.PermissionInClassDtos)
        {
            if (!teacherTeam.TeacherPermissionInClasses
                .Any(x => x.ClassId == item.ClassId
                    && x.PermissionType == item.PermissionType))
            {
                var classroom = _classRepo.GetByIdAsync(item.ClassId, cancellationToken);
                _ = classroom ?? throw new NotFoundException(_t["Classroom{0} Not Found.", item.ClassId]);

                teacherTeam.AddPermission(new TeacherPermissionInClass
                {
                    ClassId = item.ClassId,
                    TeacherTeamId = request.TeacherId,
                    PermissionType = item.PermissionType,
                });
            }
        }

        await _teacherTeamRepo.UpdateAsync(teacherTeam);

        return teacherTeam.Id;
    }
}
