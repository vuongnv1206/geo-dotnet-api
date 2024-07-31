using FSH.WebApi.Domain.Class;


namespace FSH.WebApi.Application.Class.UserStudents;
public class StudentByStIdSpec : Specification<Student>, ISingleResultSpecification
{
    public StudentByStIdSpec(Guid stId)
    {
        Query.Where(x => x.StId == stId);
    }
}
