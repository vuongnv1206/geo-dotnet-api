using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.GroupClasses.Dto;
using FSH.WebApi.Domain.Class;
using MapsterMapper;

namespace FSH.WebApi.Application.Class.GroupClasses;

public class SearchGroupClassRequest : IRequest<List<GroupClassDto>>
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

public class SearchGroupClassesRequestHandler : IRequestHandler<SearchGroupClassRequest, List<GroupClassDto>>
{
    private readonly IReadRepository<GroupClass> _repository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;

    public SearchGroupClassesRequestHandler(IReadRepository<GroupClass> repository, ICurrentUser currentUser,
                                            IStringLocalizer<SearchClassesRequestHandler> localizer) =>
        (_repository, _currentUser, _t) = (repository, currentUser, localizer);

    public async Task<List<GroupClassDto>> Handle(SearchGroupClassRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        if (!string.IsNullOrEmpty(request.Name))
        {
            return await _repository.ListAsync((ISpecification<GroupClass, GroupClassDto>)new GroupClassBySearchRequestSpec(request.Name, userId), cancellationToken);
        }
        else
        {
            return await _repository.ListAsync((ISpecification<GroupClass, GroupClassDto>)new GroupClassByUserSpec(userId), cancellationToken);
        }
    }
}