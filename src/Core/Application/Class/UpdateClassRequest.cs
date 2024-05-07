using FSH.WebApi.Application.Catalog.Products;
using FSH.WebApi.Domain.Catalog;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class;
public class UpdateClassRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string SchoolYear { get; set; }
    public Guid OwnerId { get; set; }
    public Guid GroupClassId { get; set; }
}

public class UpdateClassRequestHandler : IRequestHandler<UpdateClassRequest, Guid>
{
    public readonly IRepository<Classes> _repository;
    public readonly IRepository<GroupClass> _gcRepository;
    private readonly IStringLocalizer _t;

    public UpdateClassRequestHandler(IRepository<Classes> repository, IRepository<GroupClass> gcRepository, IStringLocalizer<UpdateClassRequestHandler> localizer) =>
    (_repository, _gcRepository, _t) = (repository, gcRepository, localizer);

    public async Task<Guid> Handle(UpdateClassRequest request, CancellationToken cancellationToken)
    {
        var classes = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = classes ?? throw new NotFoundException(_t["Classes {0} Not Found.", request.Id]);

        var gc = await _gcRepository.GetByIdAsync(request.GroupClassId, cancellationToken);

        _ = gc ?? throw new NotFoundException(_t["groupClass {0} Not Found.", request.GroupClassId]);


        var updatedClass = classes.Update(request.Name, request.SchoolYear, request.OwnerId, request.GroupClassId);

        // Add Domain Events to be raised after the commit
        classes.DomainEvents.Add(EntityUpdatedEvent.WithEntity(classes));

        await _repository.UpdateAsync(updatedClass, cancellationToken);

        return request.Id;
    }
}