using FSH.WebApi.Application.Class.UserStudents.Dto;
using FSH.WebApi.Application.Class.UserStudents.Spec;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.UserStudents;
public class SearchStudentRequest : PaginationFilter, IRequest<PaginationResponse<UserStudentDto>>
{
}

public class SearchUserStudentRequestHandler : IRequestHandler<SearchStudentRequest, PaginationResponse<UserStudentDto>>
{
    private readonly IReadRepository<Student> _repository;
    private readonly ICurrentUser _currentUser;
    public SearchUserStudentRequestHandler(
        IReadRepository<Student> repository,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PaginationResponse<UserStudentDto>> Handle(SearchStudentRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();
        var spec = new StudentBySearchSpec(request, currentUserId);
        var data = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
        return data;
    }
}
