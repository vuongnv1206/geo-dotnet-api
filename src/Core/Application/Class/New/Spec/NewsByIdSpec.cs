using FSH.WebApi.Application.Class.New.Dto;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.New.Spec;
public class NewsByIdSpec : Specification<News, NewsDto>, ISingleResultSpecification
{
    public NewsByIdSpec(DefaultIdType id)
    {
        Query.Where(n => n.Id == id);
    }
}
