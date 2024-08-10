using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.TeacherGroup;
using Mapster;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class GetGroupTeacherRequest : IRequest<GroupTeacherDto>
{
    public Guid Id { get; set; }
    public GetGroupTeacherRequest(Guid id)
    {
        Id = id;
    }
}

public class GetGroupTeacherRequestHandler : IRequestHandler<GetGroupTeacherRequest, GroupTeacherDto>
{
    private readonly IRepository<GroupTeacher> _repository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;

    public GetGroupTeacherRequestHandler(
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

    public async Task<GroupTeacherDto> Handle(GetGroupTeacherRequest request, CancellationToken cancellationToken)
    {
        var groupTeacher = await _repository.FirstOrDefaultAsync(new GroupTeacherByIdSpec(request.Id), cancellationToken);

        if (groupTeacher == null)
            throw new NotFoundException(_t["GroupTeacher{0} Not Found.", request.Id]);

        var response = groupTeacher.Adapt<GroupTeacherDto>();

        var adminGroup = await _userService.GetAsync(groupTeacher.CreatedBy.ToString(), cancellationToken);
        response.AdminGroup = adminGroup.Email ?? "";

        return response;
    }
}
