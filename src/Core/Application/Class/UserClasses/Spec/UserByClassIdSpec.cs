using FSH.WebApi.Application.Class.UserClasses.Dto;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.UserClasses.Spec;
public class UserByClassIdSpec : Specification<UserClass, UserClassDto>, ISingleResultSpecification
{
    public UserByClassIdSpec(Guid classId)
    {
        Query
            .Where(x => x.ClassesId == classId)
            .Include(x => x.Student);
    }
}
