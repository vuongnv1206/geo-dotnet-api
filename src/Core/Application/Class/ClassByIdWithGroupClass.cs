using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class;
public class ClassByIdWithGroupClass : Specification<Classes, ClassDto>, ISingleResultSpecification
{
    public ClassByIdWithGroupClass(Guid id) =>
       Query.Where(p => p.Id == id)
            .Include(p => p.GroupClass);
}
