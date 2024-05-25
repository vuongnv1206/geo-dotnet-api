using FSH.WebApi.Domain.TeacherGroup;
namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class SearchGroupTeachersRequest : PaginationFilter, IRequest<PaginationResponse<GroupTeacherDto>>
{
}

public class SearchGroupTeachersRequestHandler : IRequestHandler<SearchGroupTeachersRequest, PaginationResponse<GroupTeacherDto>>
{
    private readonly IReadRepository<GroupTeacher> _repository;
    private readonly ICurrentUser _currentUser;

    public SearchGroupTeachersRequestHandler(IReadRepository<GroupTeacher> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PaginationResponse<GroupTeacherDto>> Handle(SearchGroupTeachersRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();
        var spec = new GroupTeachersBySearchSpec(request, currentUserId);
        var data = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
        return data;
    }
}
