using FSH.WebApi.Application.Class.UserStudents.Dto;
using FSH.WebApi.Application.Class.UserStudents.Spec;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.UserStudents;
public class SearchUserStudentRequest : PaginationFilter, IRequest<PaginationResponse<UserStudentDto>>
{
}

public class SearchUserStudentRequestHandler : IRequestHandler<SearchUserStudentRequest, PaginationResponse<UserStudentDto>>
{
    private readonly IReadRepository<UserStudent> _repository;
    private readonly ICurrentUser _currentUser;
    public SearchUserStudentRequestHandler(IReadRepository<UserStudent> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PaginationResponse<UserStudentDto>> Handle(SearchUserStudentRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();
        var spec = new UserStudentBySearchSpec(request, currentUserId);
        var data = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
        return data;
    }
}
