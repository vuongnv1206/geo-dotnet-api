using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.TeacherGroup;
using Mapster;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class GetGroupTeacherGuestRequest : IRequest<GroupTeacherGuestDto>
{
    public Guid Id { get; set; }
    public GetGroupTeacherGuestRequest(Guid id)
    {
        Id = id;
    }
}

public class GetGroupTeacherGuestRequestHandler : IRequestHandler<GetGroupTeacherGuestRequest, GroupTeacherGuestDto>
{
    private readonly IRepository<GroupTeacher> _repository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;

    public GetGroupTeacherGuestRequestHandler(
        IRepository<GroupTeacher> repository,
        IStringLocalizer<GetGroupTeacherRequestHandler> t,
        ICurrentUser currentUser,
        IUserService userService)
    {
        _repository = repository;
        _t = t;
        _currentUser = currentUser;
        _userService = userService;
    }

    public async Task<GroupTeacherGuestDto> Handle(GetGroupTeacherGuestRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var groupTeacher = await _repository.FirstOrDefaultAsync(new GroupTeacherByIdSpec(request.Id), cancellationToken);

        if (groupTeacher == null)
            throw new NotFoundException(_t["GroupTeacher{0} Not Found.", request.Id]);

        var response = groupTeacher.Adapt<GroupTeacherGuestDto>();

        var adminGroup = await _userService.GetAsync(groupTeacher.CreatedBy.ToString(), cancellationToken);
        response.AdminGroup = adminGroup.Email ?? "";

        return response;
    }
}
