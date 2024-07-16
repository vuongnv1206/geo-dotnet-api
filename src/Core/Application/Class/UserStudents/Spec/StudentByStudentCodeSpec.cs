using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.UserStudents.Spec;
public class StudentByStudentCodeSpec : Specification<Student>, ISingleResultSpecification
{
    public StudentByStudentCodeSpec(string studentCode)
    {
        Query.Where(x => x.StudentCode.Equals(studentCode));
    }
}