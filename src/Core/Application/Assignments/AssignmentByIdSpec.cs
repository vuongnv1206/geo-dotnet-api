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
public class AssignmentByIdSpec : Specification<Assignment, AssignmentDto>, ISingleResultSpecification
{
    public AssignmentByIdSpec(Guid id, Guid userId)
    {
        Query
            .Where(x => x.Id == id && x.CreatedBy == userId);
    }
}
