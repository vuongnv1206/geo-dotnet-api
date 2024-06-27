using FSH.WebApi.Application.Assignments.Dtos;
using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using System.Xml.Linq;

namespace FSH.WebApi.Application.Class;
public class ClassesBySearchRequestWithGroupClassSpec : EntitiesByPaginationFilterSpec<Classes, ClassDto>
{
    public ClassesBySearchRequestWithGroupClassSpec(SearchClassesRequest request, DefaultIdType userId)
        : base(request)
    {
        Query
            .Include(p => p.GroupClass).ThenInclude(c => c.Classes)
            .Include(a => a.AssignmentClasses).ThenInclude(a => a.Assignment)
            .Include(u => u.UserClasses).ThenInclude(x => x.UserStudent)
            .Where(p => p.CreatedBy == userId);
    }
}
