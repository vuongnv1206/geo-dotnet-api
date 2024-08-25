using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.Class;
public class ClassesBySearchRequestWithGroupClassSpec : EntitiesByPaginationFilterSpec<Classes, ClassDto>
{
    public ClassesBySearchRequestWithGroupClassSpec(SearchClassesRequest request, DefaultIdType userId)
        : base(request)
    {
        Query
            .Include(p => p.GroupClass)
            .Include(a => a.AssignmentClasses)
                .ThenInclude(a => a.Assignment)
            .Include(u => u.UserClasses)
                .ThenInclude(x => x.Student)
            .Include(x => x.PaperAccesses)
                .ThenInclude(x => x.Paper)
            .Where(p => (p.CreatedBy == userId || p.UserClasses.Any(x => x.Student.StId == userId)
                    || p.TeacherPermissionInClasses.Any(tp => tp.TeacherTeam.TeacherId == userId && (tp.PermissionType == PermissionType.AssignAssignment || tp.PermissionType == PermissionType.Marking))
                    || p.GroupPermissionInClasses.Any(gp => (gp.PermissionType == PermissionType.AssignAssignment || gp.PermissionType == PermissionType.Marking) && gp.GroupTeacher.TeacherInGroups
                              .Any(tig => tig.TeacherTeam.TeacherId == userId))) &&
            (!request.GroupClassId.HasValue || p.GroupClassId == request.GroupClassId));
    }
}