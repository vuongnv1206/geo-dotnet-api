using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class;
public class ClassByIdSpec : Specification<Classes>, ISingleResultSpecification
{
    public ClassByIdSpec(DefaultIdType id)
    {
        Query.Where(b => b.Id == id);
    }
}
