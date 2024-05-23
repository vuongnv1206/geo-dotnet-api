using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.GroupClasses;
public class GroupClassByUserSpec : Specification<GroupClass, GroupClassDto>, ISingleResultSpecification
{
    public GroupClassByUserSpec(Guid userId)
    {
        Query.Where(b => b.CreatedBy == userId);
    }
}