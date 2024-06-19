using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.GroupClasses.Dto;
using FSH.WebApi.Application.Class.GroupClasses.Spec;
using FSH.WebApi.Application.Class.UserClasses;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.GroupClasses;
public class GroupClassRequest : IRequest<List<GroupClassDto>>
{
}

public class GroupClassRequestHandler : IRequestHandler<GroupClassRequest, List<GroupClassDto>>
{
    private readonly IRepository<GroupClass> _repository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;

    public GroupClassRequestHandler(IRepository<GroupClass> repository, ICurrentUser currentUser, IStringLocalizer<GetClassRequestHandler> localizer) =>
        (_repository, _currentUser, _t) = (repository, currentUser, localizer);
    public async Task<List<GroupClassDto>> Handle(GroupClassRequest request, CancellationToken cancellationToken)
    {

        var user = _currentUser.GetUserId();
        return await _repository.ListAsync(
            (ISpecification<GroupClass, GroupClassDto>)new GroupClassByUserSpec(user), cancellationToken)
        ?? throw new NotFoundException(_t["GroupClass {0} Not Found."]);
    }
}
