using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class SearchGroupTeachersRequest : PaginationFilter, IRequest<PaginationResponse<GroupTeacherDto>>
{
}

public class SearchGroupTeachersRequestHandler : IRequestHandler<SearchGroupTeachersRequest, PaginationResponse<GroupTeacherDto>>
{
    private readonly IReadRepository<GroupTeacher> _repository;

    public SearchGroupTeachersRequestHandler(IReadRepository<GroupTeacher> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<GroupTeacherDto>> Handle(SearchGroupTeachersRequest request, CancellationToken cancellationToken)
    {
        var spec = new GroupTeachersBySearchSpec(request);
        var data = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
        return data;
    }
}
