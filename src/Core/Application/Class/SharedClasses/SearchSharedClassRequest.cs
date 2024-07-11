using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.GroupClasses.Dto;
using FSH.WebApi.Application.Class.SharedClasses;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.Class.SharedClasses;
public class SearchSharedClassRequest : PaginationFilter, IRequest<PaginationResponse<SharedGroupClassDto>>
{
}

public class SearcherSharedClassRequestHandler : IRequestHandler<SearchSharedClassRequest, PaginationResponse<SharedGroupClassDto>>
{
    private readonly IRepository<GroupClass> _classRepo;
    private readonly ICurrentUser _currentUser;

    public SearcherSharedClassRequestHandler(
        IRepository<GroupClass> classRepo,
        ICurrentUser currentUser)
    {
        _classRepo = classRepo;
        _currentUser = currentUser;
    }

    public async Task<PaginationResponse<SharedGroupClassDto>> Handle(SearchSharedClassRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var spec = new SharedClassSpec(request, userId);
        var res = await _classRepo.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
        return res;
    }
}
