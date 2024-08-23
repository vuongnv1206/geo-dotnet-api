using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.TeacherGroup;
using Mapster;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class SearchSharedGroupTeachersRequest : PaginationFilter, IRequest<PaginationResponse<GroupTeacherDto>>
{
}

public class SearchSharedGroupTeachersRequestHandler : IRequestHandler<SearchSharedGroupTeachersRequest, PaginationResponse<GroupTeacherDto>>
{
    private readonly IRepository<GroupTeacher> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;

    public SearchSharedGroupTeachersRequestHandler(
        IRepository<GroupTeacher> repository,
        ICurrentUser currentUser,
        IUserService userService)
    {
        _repository = repository;
        _currentUser = currentUser;
        _userService = userService;
    }

    public async Task<PaginationResponse<GroupTeacherDto>> Handle(SearchSharedGroupTeachersRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        var spec = new SharedTeacherGroupSpec(request, userId);
        var groups = await _repository.ListAsync(spec, cancellationToken);

        var data = groups.Adapt<List<GroupTeacherDto>>();

        foreach (var group in data)
        {
            var amdin = await _userService.GetAsync(group.CreatedBy.ToString(), cancellationToken);
            group.AdminGroup = amdin.Email;
        }

        var response = new PaginationResponse<GroupTeacherDto>
            (data, groups.Count, request.PageNumber, request.PageSize);

        return response;
    }
}
