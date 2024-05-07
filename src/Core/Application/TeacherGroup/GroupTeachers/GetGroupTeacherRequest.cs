using FSH.WebApi.Application.Catalog.Brands;
using FSH.WebApi.Domain.TeacherGroup;

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
        var data = await _repository.FirstOrDefaultAsync(
             (ISpecification<GroupTeacher, GroupTeacherDto>)new GroupTeacherByIdSpec(request.Id), cancellationToken);

        if (data == null)
            throw new NotFoundException(_t["GroupTeacher{0} Not Found.", request.Id]);

        return data;
    }
}
