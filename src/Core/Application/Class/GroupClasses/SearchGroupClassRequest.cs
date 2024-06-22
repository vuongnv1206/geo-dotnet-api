using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.GroupClasses.Dto;
using FSH.WebApi.Application.Class.New.Spec;
using FSH.WebApi.Domain.Class;
using MapsterMapper;

namespace FSH.WebApi.Application.Class.GroupClasses;

public class SearchGroupClassRequest : PaginationFilter,IRequest<PaginationResponse<GroupClassDto>>
{
    public string? Name { get; set; }
}

public class GroupClassBySearchRequestSpec : Specification<GroupClass, GroupClassDto>
{
    public GroupClassBySearchRequestSpec(string? name, DefaultIdType userId)
    {
        Query
            .Include(c => c.Classes)
            .OrderBy(c => c.Name)
            .Where(c => string.IsNullOrEmpty(name) || c.Name.ToLower().Contains(name.ToLower()));

        Query.Where(x => x.CreatedBy == userId);
    }
}
public class GroupClassByUserSpec : Specification<GroupClass, GroupClassDto>
{
    public GroupClassByUserSpec(DefaultIdType userId)
    {
        Query
            .Include(c => c.Classes)
            .OrderBy(c => c.Name)
            .Where(x => x.CreatedBy == userId);
    }
}

public class SearchGroupClassesRequestHandler : IRequestHandler<SearchGroupClassRequest, PaginationResponse<GroupClassDto>>
{
    private readonly IReadRepository<GroupClass> _repository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;

    public SearchGroupClassesRequestHandler(IReadRepository<GroupClass> repository, ICurrentUser currentUser,
                                            IStringLocalizer<SearchClassesRequestHandler> localizer) =>
        (_repository, _currentUser, _t) = (repository, currentUser, localizer);

    public async Task<PaginationResponse<GroupClassDto>> Handle(SearchGroupClassRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var spec = new GroupClassBySearchRequestSpec(request.Name, userId);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}