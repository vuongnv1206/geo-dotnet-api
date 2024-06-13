using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class;
public class ClassByGroupClassSpec : Specification<Classes>, ISingleResultSpecification
{
    public ClassByGroupClassSpec(Guid groupClassId)
    {
        Query.Where(x => x.GroupClassId == groupClassId);
    }
}
