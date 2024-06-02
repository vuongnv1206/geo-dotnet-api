using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class;
public class ClassByIdWithGroupClassSpec : Specification<Classes, ClassDto>, ISingleResultSpecification
{
    public ClassByIdWithGroupClassSpec(DefaultIdType id) =>
       Query.Where(p => p.Id == id)
            .Include(p => p.GroupClass);
}
