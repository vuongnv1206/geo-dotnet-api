using FSH.WebApi.Application.Catalog.Products;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class;
public class ClassByIdWithGroupClass : Specification<Classes, ClassDto>, ISingleResultSpecification
{
    public ClassByIdWithGroupClass(Guid id) =>
       Query.Where(p => p.Id == id)
            .Include(p => p.GroupClass);
}
