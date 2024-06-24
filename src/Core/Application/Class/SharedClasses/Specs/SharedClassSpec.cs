using FSH.WebApi.Application.Class.GroupClasses.Dto;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.SharedClasses;
public class SharedClassSpec : EntitiesByPaginationFilterSpec<GroupClass, GroupClassDto>
{
    public SharedClassSpec(SearchSharedClassRequest request, Guid userId)
        : base(request)
    {
        Query.Include(gc => gc.Classes)
                .ThenInclude(c => c.TeacherPermissionInClasses)
                    .ThenInclude(tp => tp.TeacherTeam)
            .Include(gc => gc.Classes)
                .ThenInclude(c => c.GroupPermissionInClasses)
                    .ThenInclude(gp => gp.GroupTeacher)
                        .ThenInclude(gt => gt.TeacherInGroups)
                            .ThenInclude(tg => tg.TeacherTeam)
            .Where(gc => gc.Classes
                .Any(c => c.TeacherPermissionInClasses.Any(tp => tp.TeacherTeam.TeacherId == userId)
                        || c.GroupPermissionInClasses.Any(gp => gp.GroupTeacher.TeacherInGroups
                                                    .Any(tig => tig.TeacherTeam.TeacherId == userId))));
    }
}
