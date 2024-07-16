using FSH.WebApi.Application.Class.UserStudents.Dto;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.UserStudents.Spec;
public class StudentDetailByIdSpec : Specification<Student, UserStudentDto>, ISingleResultSpecification
{
    public StudentDetailByIdSpec(Guid studentId)
    {
        Query.Where(x => x.Id == studentId);
    }
}