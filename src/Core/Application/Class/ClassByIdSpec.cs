using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class;
public class ClassByIdSpec : Specification<Classes>, ISingleResultSpecification
{
    public ClassByIdSpec(Guid id, Guid userId)
    {
        Query.Where(b => b.Id == id).Include(x => x.UserClasses)
            .Include(c => c.TeacherPermissionInClasses)
                .ThenInclude(tp => tp.TeacherTeam)
            .Include(c => c.GroupPermissionInClasses)
                .ThenInclude(gp => gp.GroupTeacher)
                    .ThenInclude(gt => gt.TeacherInGroups)
                        .ThenInclude(tg => tg.TeacherTeam)
            .Where(c => c.CreatedBy == userId
                        || c.TeacherPermissionInClasses.Any(tp => tp.TeacherTeam.TeacherId == userId)
                        || c.GroupPermissionInClasses.Any(gp => gp.GroupTeacher.TeacherInGroups
                                .Any(tig => tig.TeacherTeam.TeacherId == userId)));
    }
}
