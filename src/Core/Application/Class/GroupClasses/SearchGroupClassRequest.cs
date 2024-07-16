using FSH.WebApi.Application.Class.GroupClasses.Dto;
using FSH.WebApi.Application.Class.GroupClasses.Spec;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.GroupClasses;

public class SearchGroupClassRequest : PaginationFilter, IRequest<PaginationResponse<GroupClassDto>>
{
    public ClassroomQueryType QueryType { get; set; }
}

public enum ClassroomQueryType
{
    All = 0,
    MyClass = 1,
    SharedClass = 2
}

public class SearchGroupClassesRequestHandler : IRequestHandler<SearchGroupClassRequest, PaginationResponse<GroupClassDto>>
{
    private readonly IReadRepository<GroupClass> _repository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;

    public SearchGroupClassesRequestHandler(
        IReadRepository<GroupClass> repository,
        ICurrentUser currentUser,
        IStringLocalizer<SearchClassesRequestHandler> localizer) =>
        (_repository, _currentUser, _t) = (repository, currentUser, localizer);

    public async Task<PaginationResponse<GroupClassDto>> Handle(SearchGroupClassRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var spec = new GroupClassBySearchRequestSpec(request, userId);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}