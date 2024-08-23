using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.JoinGroups;
public class JoinGroupRequestById : Specification<JoinGroupTeacherRequest>, ISingleResultSpecification
{
    public JoinGroupRequestById(Guid id)
    {
        Query.Where(x => x.Id == id)
            .Include(x => x.GroupTeacher);
    }
}
