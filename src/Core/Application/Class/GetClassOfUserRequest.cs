using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class;
public class GetClassOfUserRequest : IRequest<List<ClassDto>>
{
}

public class GetClassOfUserRequestHandler : IRequestHandler<GetClassOfUserRequest, List<ClassDto>>
{
    private readonly IRepository<Classes> _repository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;

    public GetClassOfUserRequestHandler(IRepository<Classes> repository, ICurrentUser currentUser, IStringLocalizer<GetClassRequestHandler> localizer) =>
        (_repository, _currentUser, _t) = (repository, currentUser, localizer);
    public async Task<List<ClassDto>> Handle(GetClassOfUserRequest request, CancellationToken cancellationToken)
    {

        var user = _currentUser.GetUserId();
        return await _repository.ListAsync(
            (ISpecification<Classes, ClassDto>)new ClassByUserSpec(user), cancellationToken)
        ?? throw new NotFoundException(_t["Classes {0} Not Found.", user]);
    }
}
