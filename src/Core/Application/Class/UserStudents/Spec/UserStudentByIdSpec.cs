using FSH.WebApi.Application.TeacherGroup.TeacherTeams;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.UserStudents.Spec;
public class UserStudentByIdSpec : Specification<UserStudent>, ISingleResultSpecification
{
    public UserStudentByIdSpec(Guid id)
    {
        Query.Include(x => x.UserClasses)
             .Where(x => x.Id == id);
    }
}
