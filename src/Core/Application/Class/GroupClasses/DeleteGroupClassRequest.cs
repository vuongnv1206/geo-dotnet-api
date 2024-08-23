using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.GroupClasses;
public class DeleteGroupClassRequest : IRequest<Guid>
{
    public Guid Id { get; set; }

    public DeleteGroupClassRequest(Guid id) => Id = id;
}

public class DeleteGroupClassRequestHandler : IRequestHandler<DeleteGroupClassRequest, Guid>
{
    private readonly IRepositoryWithEvents<GroupClass> _groupClassRepo;
    private readonly IReadRepository<Classes> _classRepo;
    private readonly IRepositoryWithEvents<Classes> _classEventRepo;

    private readonly IStringLocalizer _t;

    public DeleteGroupClassRequestHandler(IRepositoryWithEvents<GroupClass> groupClassRepo, IReadRepository<Classes> classRepo,
                                          IStringLocalizer<DeleteGroupClassRequestHandler> localizer, IRepositoryWithEvents<Classes> classEventRepo) =>
        (_groupClassRepo, _classRepo, _t, _classEventRepo) = (groupClassRepo, classRepo, localizer, classEventRepo);

    public async Task<Guid> Handle(DeleteGroupClassRequest request, CancellationToken cancellationToken)
    {


        var groupClass = await _groupClassRepo.FirstOrDefaultAsync(new GroupClassByIdSpec(request.Id), cancellationToken);
        _ = groupClass ?? throw new NotFoundException(_t["GroupClass {0} Not Found."]);

        await _groupClassRepo.DeleteAsync(groupClass, cancellationToken);

        return request.Id;
    }
}