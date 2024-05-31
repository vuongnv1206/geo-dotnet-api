using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.TeacherTeams;
public class SearchTeacherTeamsRequest : PaginationFilter, IRequest<PaginationResponse<TeacherTeamDto>>
{
}

public class SearchTeacherTeamsRequestHandler : IRequestHandler<SearchTeacherTeamsRequest, PaginationResponse<TeacherTeamDto>>
{
    private readonly IReadRepository<TeacherTeam> _repository;
    private readonly ICurrentUser _currentUser;
    public SearchTeacherTeamsRequestHandler(IReadRepository<TeacherTeam> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PaginationResponse<TeacherTeamDto>> Handle(SearchTeacherTeamsRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();
        var spec = new TeacherTeamBySearchSpec(request, currentUserId);
        var data = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
        return data;
    }
}
