using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.GroupClasses;
public class GetGroupClassRequest : IRequest<List<GroupClassOfClassDto>>
{
}

public class GetGroupClassRequestHandler : IRequestHandler<GetGroupClassRequest, List<GroupClassOfClassDto>>
{
    private readonly IRepository<GroupClass> _repository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;

    public GetGroupClassRequestHandler(IRepository<GroupClass> repository, ICurrentUser currentUser, IStringLocalizer<GetGroupClassRequestHandler> localizer) =>
        (_repository, _currentUser, _t) = (repository, currentUser, localizer);
    public async Task<List<GroupClassOfClassDto>> Handle(GetGroupClassRequest request, CancellationToken cancellationToken)
    {
        var user = _currentUser.GetUserId();
        return await _repository.ListAsync(
                       (ISpecification<GroupClass, GroupClassOfClassDto>)new GroupClassOfClassSpec(user), cancellationToken)
        ?? throw new NotFoundException(_t["GroupClasses {0} Not Found.", user]);
    }
}   
