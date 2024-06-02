using FSH.WebApi.Application.Class.GroupClasses.Dto;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.GroupClasses.Spec;
public class GroupClassOfClassSpec : Specification<GroupClass, GroupClassOfClassDto>, ISingleResultSpecification
{
    public GroupClassOfClassSpec(DefaultIdType userId)
    {
        Query.Where(a => a.CreatedBy == userId).Include(x => x.Classes);
    }
}