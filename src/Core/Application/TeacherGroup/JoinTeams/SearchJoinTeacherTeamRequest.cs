using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.TeacherGroup.JoinGroups;
using FSH.WebApi.Domain.TeacherGroup;
using Mapster;

namespace FSH.WebApi.Application.TeacherGroup.JoinTeams;
public class SearchJoinTeacherTeamRequest : PaginationFilter, IRequest<PaginationResponse<JoinTeacherTeamRequestDto>>
{
    public RequestStatus Status { get; set; }
}

public class SearchJoinTeacherTeamRequestHandler : IRequestHandler<SearchJoinTeacherTeamRequest, PaginationResponse<JoinTeacherTeamRequestDto>>
{
    private readonly IRepository<JoinTeacherTeamRequest> _joinTeacherTeamRepo;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;
    private readonly IUserService _userService;

    public SearchJoinTeacherTeamRequestHandler(
        IRepository<JoinTeacherTeamRequest> joinTeacherTeamRepo,
        ICurrentUser currentUser,
        IStringLocalizer<SearchJoinTeacherTeamRequestHandler> t,
        IUserService userService)
    {
        _joinTeacherTeamRepo = joinTeacherTeamRepo;
        _currentUser = currentUser;
        _t = t;
        _userService = userService;
    }

    public async Task<PaginationResponse<JoinTeacherTeamRequestDto>> Handle(SearchJoinTeacherTeamRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        var joinTeacherTeamRequests = new List<JoinTeacherTeamRequest>();
        int totalItem = 0;

        switch (request.Status)
        {
            case RequestStatus.Sent:
                var specSent = new JoinTeamRequestSentSpec(request, userId);
                joinTeacherTeamRequests = await _joinTeacherTeamRepo.ListAsync(specSent, cancellationToken);
                totalItem = await _joinTeacherTeamRepo.CountAsync(specSent, cancellationToken);
                break;
            case RequestStatus.Received:
                var specReceived = new JoinTeamRequestReceivedSpec(request, userId);
                joinTeacherTeamRequests = await _joinTeacherTeamRepo.ListAsync(specReceived, cancellationToken);
                totalItem = await _joinTeacherTeamRepo.CountAsync(specReceived, cancellationToken);
                break;
            default:
                throw new BadRequestException(_t["Miss status filter."]);
        }

        var response = joinTeacherTeamRequests.Adapt<List<JoinTeacherTeamRequestDto>>();

        foreach(var item in response)
        {
            item.AdminTeamEmail = await GetEmailUser(item.AdminTeamId, cancellationToken);
            item.SenderFullName = await GetFullNameUser(item.CreateBy, cancellationToken);
        }

        return new PaginationResponse<JoinTeacherTeamRequestDto>
        (
            response,
            totalItem,
            request.PageNumber,
            request.PageSize
        );
    }

    private async Task<string> GetEmailUser(Guid userId, CancellationToken cancellationToken)
    {
        var userDetail = await _userService.GetAsync(userId.ToString(), cancellationToken);

        return userDetail.Email;
    }

    private async Task<string> GetFullNameUser(Guid userId, CancellationToken cancellationToken)
    {
        var userDetail = await _userService.GetAsync(userId.ToString(), cancellationToken);

        return $"{userDetail.FirstName} {userDetail.LastName}";
    }
}
