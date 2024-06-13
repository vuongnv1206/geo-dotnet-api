using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class;
public class ClassByUserSpec : Specification<Classes>, ISingleResultSpecification
{
    public ClassByUserSpec(DefaultIdType userId)
    {

        Query.Include(a => a.AssignmentClasses).ThenInclude(a => a.Assignment).Where(a => a.OwnerId == userId);
        //Query.Where(b => b.OwnerId == userId);
    }
}
