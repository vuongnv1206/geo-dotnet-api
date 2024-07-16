using FSH.WebApi.Application.Class.GroupClasses.Dto;
using FSH.WebApi.Domain.Class;
using Mapster;

namespace FSH.WebApi.Application.Class.GroupClasses.Spec;
internal class GroupClassBySearchRequestSpec : Specification<GroupClass, GroupClassDto>, ISingleResultSpecification
{
    public GroupClassBySearchRequestSpec(SearchGroupClassRequest request, DefaultIdType userId)
        : base((IInMemorySpecificationEvaluator)request)
    {
        Query.Include(gc => gc.Classes)
                .ThenInclude(c => c.TeacherPermissionInClasses)
                    .ThenInclude(tp => tp.TeacherTeam)
            .Include(gc => gc.Classes)
                .ThenInclude(c => c.GroupPermissionInClasses)
                    .ThenInclude(gp => gp.GroupTeacher)
                        .ThenInclude(gt => gt.TeacherInGroups)
                            .ThenInclude(tg => tg.TeacherTeam)
            .Include(gc => gc.Classes)
                .ThenInclude(c => c.UserClasses)
                    .ThenInclude(uc => uc.Student)
            .Where(gc => gc.CreatedBy == userId
                        || gc.Classes.Any(c => c.TeacherPermissionInClasses.Any(tp => tp.TeacherTeam.TeacherId == userId)
                        || c.GroupPermissionInClasses.Any(gp => gp.GroupTeacher.TeacherInGroups.Any(tig => tig.TeacherTeam.TeacherId == userId))
                        || c.UserClasses.Any(uc => uc.Student.StId == userId)));

        if (request.QueryType == ClassroomQueryType.MyClass)
        {
            Query.Where(gc => gc.CreatedBy == userId);
        }
        else if (request.QueryType == ClassroomQueryType.SharedClass)
        {
            Query.Where(gc => gc.Classes
                .Any(c => c.TeacherPermissionInClasses.Any(tp => tp.TeacherTeam.TeacherId == userId)
                        || c.GroupPermissionInClasses.Any(gp => gp.GroupTeacher.TeacherInGroups
                                                    .Any(tig => tig.TeacherTeam.TeacherId == userId))));
        }

        Query.Select(gc => new GroupClassDto
        {
            Id = gc.Id,
            Name = gc.Name,
            Classes = gc.Classes
            .Where(c => c.CreatedBy == userId
                        || c.TeacherPermissionInClasses.Any(tp => tp.TeacherTeam.TeacherId == userId)
                        || c.GroupPermissionInClasses.Any(gp => gp.GroupTeacher.TeacherInGroups.Any(tig => tig.TeacherTeam.TeacherId == userId))
                        || c.UserClasses.Any(uc => uc.Student.StId == userId)).ToList().Adapt<List<ClassViewListDto>>(),
            CreatedBy = gc.CreatedBy
        });
    }
}
