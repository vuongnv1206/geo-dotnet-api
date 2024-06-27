using FSH.WebApi.Application.Class.New.Dto;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New.Spec;
public class PostBySearchRequestWithClass : Specification<Post>
{
    public PostBySearchRequestWithClass(Guid? classId)
    {
        Query
            .Include(p => p.Classes)
            .Where(p => p.ClassesId.Equals(classId));
    }
}
