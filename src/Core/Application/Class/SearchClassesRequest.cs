using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class;
public class SearchClassesRequest : PaginationFilter, IRequest<PaginationResponse<ClassDto>>
{
    public Guid? GroupClassId { get; set; }
}

public class SearchClassesRequestHandler : IRequestHandler<SearchClassesRequest, PaginationResponse<ClassDto>>
{
    private readonly IReadRepository<Classes> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;
    private readonly IUserService _userService;

    public SearchClassesRequestHandler(
        IReadRepository<Classes> repository,
        ICurrentUser currentUser,
        IStringLocalizer<SearchClassesRequestHandler> localizer,
        IUserService userService)
    {
        _repository = repository;
        _currentUser = currentUser;
        _t = localizer;
        _userService = userService;
    }

    public async Task<PaginationResponse<ClassDto>> Handle(SearchClassesRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var spec = new ClassesBySearchRequestWithGroupClassSpec(request, userId);
        var data = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);

        // Get owner details for each Question
        foreach (var class1 in data.Data)
        {
            try
            {
                var user = await _userService.GetAsync(class1.OwnerId.ToString(), cancellationToken);
                if (user != null)
                {
                    class1.Owner = user;
                }
            }
            catch
            {

            }
        }

        return data;
    }
}
