using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.GroupClasses;
public class GroupClassByNameSpec : Specification<GroupClass>, ISingleResultSpecification
{
    public GroupClassByNameSpec(string name) {
        Query.Where(b => b.Name.ToLower().Equals(name.ToLower().Trim()));
    }

}
