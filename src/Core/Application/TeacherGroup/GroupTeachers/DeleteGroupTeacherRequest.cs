
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class DeleteGroupTeacherRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public DeleteGroupTeacherRequest(Guid id) => Id = id;
}

public class DeleteGroupTeacherRequestHandler : IRequestHandler<DeleteGroupTeacherRequest, Guid>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<GroupTeacher> _groupRepo;

    private readonly IStringLocalizer _t;

    public DeleteGroupTeacherRequestHandler(IRepositoryWithEvents<GroupTeacher> groupRepo,  IStringLocalizer<DeleteGroupTeacherRequestHandler> localizer) =>
        (_groupRepo, _t) = (groupRepo, localizer);

    public async Task<Guid> Handle(DeleteGroupTeacherRequest request, CancellationToken cancellationToken)
    {
        var group = await _groupRepo.GetByIdAsync(request.Id, cancellationToken);

        _ = group ?? throw new NotFoundException(_t["GroupTeacher {0} Not Found."]);

        await _groupRepo.DeleteAsync(group, cancellationToken);

        return request.Id;
    }
}