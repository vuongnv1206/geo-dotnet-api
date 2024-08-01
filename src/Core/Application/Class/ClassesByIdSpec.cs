using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class;
public class ClassesByIdSpec : Specification<Classes>, ISingleResultSpecification
{
    public ClassesByIdSpec(Guid id)
    {
        Query
            .Include(x => x.AssignmentClasses)
            .Include(x => x.UserClasses).ThenInclude(x => x.Student)
            .Where(x => x.Id == id);
    }
}
