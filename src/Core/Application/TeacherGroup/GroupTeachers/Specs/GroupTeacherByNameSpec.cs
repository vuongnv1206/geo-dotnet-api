using FSH.WebApi.Domain.TeacherGroup;
namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class GroupTeacherByNameSpec : Specification<GroupTeacher>, ISingleResultSpecification
{
    public GroupTeacherByNameSpec(string name)
    {
        Query.Where(b => b.Name == name);
    }
}
