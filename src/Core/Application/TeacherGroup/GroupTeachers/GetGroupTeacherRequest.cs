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

    public GetGroupTeacherRequestHandler(IRepository<GroupTeacher> repository, IStringLocalizer<GetGroupTeacherRequestHandler> localizer) => (_repository, _t) = (repository, localizer);

    public async Task<GroupTeacherDto> Handle(GetGroupTeacherRequest request, CancellationToken cancellationToken)
    {
        var groupTeacher = await _repository.FirstOrDefaultAsync(new GroupTeacherByIdSpec(request.Id), cancellationToken);

        if (groupTeacher == null)
            throw new NotFoundException(_t["GroupTeacher{0} Not Found.", request.Id]);

        return groupTeacher.Adapt<GroupTeacherDto>();
    }
}
