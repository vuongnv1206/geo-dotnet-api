using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        var classes = await _classRepo.ListAsync(new ClassByGroupClassSpec(request.Id), cancellationToken);
        if (classes != null)
        {
            foreach (var c in classes)
            {
                c.UpdateGroupClassId(null);
                await _classEventRepo.UpdateAsync(c, cancellationToken);
            }
        }

        var groupClass = await _groupClassRepo.GetByIdAsync(request.Id, cancellationToken);

        _ = groupClass ?? throw new NotFoundException(_t["GroupClass {0} Not Found."]);

        await _groupClassRepo.DeleteAsync(groupClass, cancellationToken);

        return request.Id;
    }
}