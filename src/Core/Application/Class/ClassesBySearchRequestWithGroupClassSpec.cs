using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using System.Xml.Linq;

namespace FSH.WebApi.Application.Class;
public class ClassesBySearchRequestWithGroupClassSpec : Specification<Classes, ClassDto>
{
    public ClassesBySearchRequestWithGroupClassSpec(string? name, DefaultIdType userId)
    {
        Query
            .Include(p => p.GroupClass)
            .Where(p => (string.IsNullOrEmpty(name) || p.Name.ToLower().Contains(name.ToLower())));

        Query.Where(p => p.CreatedBy == userId);
    }
}
