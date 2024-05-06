using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class GroupTeacherByIdSpec : Specification<GroupTeacher,GroupTeacherDto>,ISingleResultSpecification
{
    public GroupTeacherByIdSpec(Guid id)
    {
        Query.Where(x => x.Id == id);
    }
}
