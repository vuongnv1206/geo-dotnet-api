using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.UserStudents.Spec;
public class StudentByStudentCodeSpec : Specification<Student>, ISingleResultSpecification
{
    public StudentByStudentCodeSpec(string studentCode)
    {
        Query.Where(x => x.StudentCode.Equals(studentCode));
    }
}