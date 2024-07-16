using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class;
public class ClassByUserSpec : Specification<Classes, ClassDto>, ISingleResultSpecification
{
    public ClassByUserSpec(DefaultIdType userId)
    {

        Query
            .Include(a => a.AssignmentClasses)
            .ThenInclude(a => a.Assignment)
            .Where(a => a.CreatedBy == userId);
    }
}
