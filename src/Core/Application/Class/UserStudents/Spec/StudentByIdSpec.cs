using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.UserStudents.Spec;
public class StudentByIdSpec : Specification<Student>, ISingleResultSpecification
{
    public StudentByIdSpec(Guid id)
    {
        Query.Where(x => x.Id == id);
    }
}
