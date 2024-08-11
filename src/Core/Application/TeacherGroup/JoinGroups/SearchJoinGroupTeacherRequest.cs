using FSH.WebApi.Application.Common.Models;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.TeacherGroup;
using Mapster;

namespace FSH.WebApi.Application.TeacherGroup.JoinGroups;
public class SearchJoinGroupTeacherRequest : PaginationFilter, IRequest<PaginationResponse<JoinGroupTeacherRequestDto>>
{
    public RequestStatus Status { get; set; }
}

public class SearchJoinGroupTeacherRequestHandler : IRequestHandler<SearchJoinGroupTeacherRequest, PaginationResponse<JoinGroupTeacherRequestDto>>
{
    private readonly IRepository<JoinGroupTeacherRequest> _joinGroupTeacherRepo;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;
    private readonly IUserService _userService;

    public SearchJoinGroupTeacherRequestHandler(
        IRepository<JoinGroupTeacherRequest> joinGroupTeacherRepo,
        ICurrentUser currentUser,
        IStringLocalizer<SearchJoinGroupTeacherRequestHandler> t,
        IUserService userService)
    {
        _joinGroupTeacherRepo = joinGroupTeacherRepo;
        _currentUser = currentUser;
        _t = t;
        _userService = userService;
    }

    public async Task<PaginationResponse<JoinGroupTeacherRequestDto>> Handle(SearchJoinGroupTeacherRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        var joinGroupRequests = new List<JoinGroupTeacherRequest>();
        int totalItem = 0;
        switch (request.Status)
        {
            case RequestStatus.Sent:
                var specSent = new JoinGroupRequestSentSpec(request, userId);
                joinGroupRequests = await _joinGroupTeacherRepo.ListAsync(specSent, cancellationToken);
                totalItem = await _joinGroupTeacherRepo.CountAsync(specSent, cancellationToken);
                break;
            case RequestStatus.Received:
                var specReceived = new JoinGroupRequestReceivedSpec(request, userId);
                joinGroupRequests = await _joinGroupTeacherRepo.ListAsync(specReceived, cancellationToken);
                totalItem = await _joinGroupTeacherRepo.CountAsync(specReceived, cancellationToken);
                break;
            default:
                throw new BadRequestException(_t["Miss status filter."]);
        }

        var response = joinGroupRequests.Adapt<List<JoinGroupTeacherRequestDto>>();

        foreach(var item in response)
        {
            var adminGroup = await _userService.GetAsync(item.ReceiverId.ToString(), cancellationToken);
            item.ReceiverEmail = adminGroup.Email;
        }

        return new PaginationResponse<JoinGroupTeacherRequestDto>
        (
            response,
            request.PageSize,
            request.PageNumber,
            totalItem
        );
    }
}
