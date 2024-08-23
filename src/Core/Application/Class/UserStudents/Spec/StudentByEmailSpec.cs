using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.UserStudents;
public class StudentByEmailSpec : Specification<Student>
{
    public StudentByEmailSpec(string email)
    {
        Query.Where(x => x.Email == email);
    }
}
