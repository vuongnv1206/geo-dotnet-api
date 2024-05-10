using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New;
public class NewsByIdSpec : Specification<News, NewsDto>, ISingleResultSpecification
{
    public NewsByIdSpec(Guid id) {
        Query.Where(n => n.Id == id);
    }
}
