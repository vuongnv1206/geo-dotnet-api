using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.GroupClasses;
public class GroupClassOfClassSpec : Specification<GroupClass, GroupClassOfClassDto>, ISingleResultSpecification
{
    public GroupClassOfClassSpec(Guid userId)
    {
        Query.Where(a => a.CreatedBy == userId).Include(x => x.Classes);
    }
}