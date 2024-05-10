

using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.PermissionClasses;
public class GetGroupTeacherWithPermissionRequest : IRequest<GroupTeacherDto>
{
    public Guid Id { get; set; }
    public GetGroupTeacherWithPermissionRequest(Guid id)
    {
        Id = id;
    }
}


public class GetGroupTeacherWithPermissionHandler : IRequestHandler<GetGroupTeacherWithPermissionRequest, GroupTeacherDto>
{
    private readonly IRepository<GroupTeacher> _repository;
    private readonly IStringLocalizer _t;

    public GetGroupTeacherWithPermissionHandler(IRepository<GroupTeacher> repository, IStringLocalizer<GetGroupTeacherWithPermissionHandler> t)
    {
        _repository = repository;
        _t = t;
    }

    public async Task<GroupTeacherDto> Handle(GetGroupTeacherWithPermissionRequest request, CancellationToken cancellationToken)
    {
        var data = await _repository.FirstOrDefaultAsync(
             (ISpecification<GroupTeacher, GroupTeacherDto>)new GroupTeacherByIdSpec(request.Id), cancellationToken);

        if (data == null)
            throw new NotFoundException(_t["GroupTeacher{0} Not Found.", request.Id]);

        return data;
    }
}