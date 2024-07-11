using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.PermissionClasses;
public class GroupPermissionClassByUserIdAndClassIdSpec
    : Specification<GroupPermissionInClass, PermissionInClassDto>, ISingleResultSpecification
{
    public GroupPermissionClassByUserIdAndClassIdSpec(Guid userId, Guid classId)
    {
        Query.Where(x => x.ClassId == classId)
            .Include(x => x.GroupTeacher)
                .ThenInclude(gt => gt.TeacherInGroups)
                    .ThenInclude(tig => tig.TeacherTeam)
            .Where(x => x.GroupTeacher.TeacherInGroups
                .Any(tig => tig.TeacherTeam.TeacherId == userId));
    }
}
