using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Domain.TeacherGroup;


namespace FSH.WebApi.Application.TeacherGroup.PermissionClasses.Specs;
public class GroupTeacherByIdWithPermissionSpec : Specification<GroupTeacher>, ISingleResultSpecification
{
    public GroupTeacherByIdWithPermissionSpec(Guid id)
    {
        Query
           .Where(p => p.Id == id)
           .Include(p => p.GroupPermissionInClasses);
    }
}
