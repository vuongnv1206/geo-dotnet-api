using FSH.WebApi.Application.Class.GroupClasses.Dto;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.GroupClasses.Spec;
public class GroupClassByUserSpec : Specification<GroupClass, GroupClassDto>, ISingleResultSpecification
{
    public GroupClassByUserSpec(DefaultIdType userId)
    {
        Query.Where(b => b.CreatedBy == userId)
            .Include(x => x.Classes);
    }
}