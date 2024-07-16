using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.PermissionClasses;
public class TeacherPermissionCLassByUserIdAndClassIdSpec
    : Specification<TeacherPermissionInClass, PermissionInClassDto>, ISingleResultSpecification
{
    public TeacherPermissionCLassByUserIdAndClassIdSpec(Guid userId, Guid classId)
    {
        Query
            .Where(x => x.ClassId == classId)
            .Include(x => x.TeacherTeam)
            .Where(x => x.TeacherTeam.TeacherId == userId);
    }
}
