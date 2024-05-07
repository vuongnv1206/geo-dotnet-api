using FSH.WebApi.Application.Catalog.Brands;
using FSH.WebApi.Application.Catalog.Products;
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

    public DeleteGroupClassRequest(Guid id ) => Id = id;
}

public class DeleteGroupClassRequestHandler : IRequestHandler<DeleteGroupClassRequest, Guid>
{
    private readonly IRepositoryWithEvents<GroupClass> _groupClassRepo;
    private readonly IReadRepository<Classes> _classRepo;
    private readonly IStringLocalizer _t;

    public DeleteGroupClassRequestHandler(IRepositoryWithEvents<GroupClass> groupClassRepo, IReadRepository<Classes> classRepo, IStringLocalizer<DeleteGroupClassRequestHandler> localizer) =>
        (_groupClassRepo, _classRepo, _t) = (groupClassRepo, classRepo, localizer);

    public async Task<Guid> Handle(DeleteGroupClassRequest request, CancellationToken cancellationToken)
    {
        if (await _classRepo.AnyAsync(new ClassByGroupClassSpec(request.Id), cancellationToken))
        {
            throw new ConflictException(_t["GroupClass cannot be deleted as it's being used."]);
        }

        var groupClass = await _groupClassRepo.GetByIdAsync(request.Id, cancellationToken);

        _ = groupClass ?? throw new NotFoundException(_t["GroupClass {0} Not Found."]);

        await _groupClassRepo.DeleteAsync(groupClass, cancellationToken);

        return request.Id;
    }
}