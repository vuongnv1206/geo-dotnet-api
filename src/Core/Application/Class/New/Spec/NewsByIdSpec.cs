using FSH.WebApi.Application.Class.New.Dto;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New.Spec;
public class NewsByIdSpec : Specification<News, NewsDto>, ISingleResultSpecification
{
    public NewsByIdSpec(DefaultIdType id)
    {
        Query.Where(n => n.Id == id);
    }
}
