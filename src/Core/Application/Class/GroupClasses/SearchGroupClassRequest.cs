using FSH.WebApi.Application.Catalog.Brands;
using FSH.WebApi.Application.Class.GroupClasses.Dto;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.GroupClasses;

public class SearchGroupClassRequest : PaginationFilter, IRequest<PaginationResponse<GroupClassDto>>
{
}

public class GroupClassBySearchRequestSpec : EntitiesByPaginationFilterSpec<GroupClass, GroupClassDto>
{
    public GroupClassBySearchRequestSpec(SearchGroupClassRequest request)
       : base(request) =>
       Query.OrderBy(c => c.Name, !request.HasOrderBy());
}

public class SearchGroupClassesRequestHandler : IRequestHandler<SearchGroupClassRequest, PaginationResponse<GroupClassDto>>
{
    private readonly IReadRepository<GroupClass> _repository;

    public SearchGroupClassesRequestHandler(IReadRepository<GroupClass> repository) => _repository = repository;

    public async Task<PaginationResponse<GroupClassDto>> Handle(SearchGroupClassRequest request, CancellationToken cancellationToken)
    {
        var spec = new GroupClassBySearchRequestSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
    }
}