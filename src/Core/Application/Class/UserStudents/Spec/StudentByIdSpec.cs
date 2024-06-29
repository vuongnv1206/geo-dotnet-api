using FSH.WebApi.Application.Class.UserStudents.Dto;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.UserStudents.Spec;
public class StudentByIdSpec : Specification<Student>, ISingleResultSpecification
{
    public StudentByIdSpec(Guid id)
    {
        Query.Where(x => x.Id == id);
    }
}
