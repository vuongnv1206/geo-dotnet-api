using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.GroupClasses;
public class GetClassByGroupClassRequest : IRequest<List<Classes>>
{
    public Guid GroupClassId { get; set; }
    public GetClassByGroupClassRequest(Guid groupClassId) => GroupClassId = groupClassId;
}

public class GetClassByGroupClassRequestHandler : IRequestHandler<GetClassByGroupClassRequest, List<Classes>>
{
    private readonly IRepository<Classes> _repository;
    private readonly IStringLocalizer<GetClassByGroupClassRequestHandler> _t;

    public GetClassByGroupClassRequestHandler(IRepository<Classes> repository, IStringLocalizer<GetClassByGroupClassRequestHandler> localizer) =>
        (_repository, _t) = (repository, localizer);
    public async Task<List<Classes>> Handle(GetClassByGroupClassRequest request, CancellationToken cancellationToken)
    {
        return await _repository.ListAsync((ISpecification<Classes>)new ClassByGroupClassSpec(request.GroupClassId), cancellationToken)
        ?? throw new NotFoundException(_t["Get Class By GroupClassId {0} Not Found.", request.GroupClassId]);
    } 
}
