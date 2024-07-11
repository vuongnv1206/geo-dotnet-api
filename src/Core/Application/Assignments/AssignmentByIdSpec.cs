using FSH.WebApi.Application.Assignments.Dtos;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.TeacherGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Assignments;
public class AssignmentByIdSpec : Specification<Assignment>, ISingleResultSpecification
{
    public AssignmentByIdSpec(Guid id)
    {
        Query
            .Include(a => a.AssignmentClasses)
            .Include(a => a.AssignmentStudents).ThenInclude(x => x.Student)
            .Where(x => x.Id == id);
    }
}
