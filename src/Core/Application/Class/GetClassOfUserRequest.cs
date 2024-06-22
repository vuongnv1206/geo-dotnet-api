using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.UserClasses;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class;
public class GetClassOfUserRequest : IRequest<List<ClassDto>>
{
}

public class GetClassOfUserRequestHandler : IRequestHandler<GetClassOfUserRequest, List<ClassDto>>
{
    private readonly IRepository<Classes> _repository;
    private readonly IStringLocalizer _t;
    private readonly IUserClassesRepository _userClassesRepository;
    private readonly ICurrentUser _currentUser;

    public GetClassOfUserRequestHandler(IRepository<Classes> repository, ICurrentUser currentUser,
                                        IStringLocalizer<GetClassRequestHandler> localizer, IUserClassesRepository userClassesRepository) =>
        (_repository, _currentUser, _t, _userClassesRepository) = (repository, currentUser, localizer, userClassesRepository);
    public async Task<List<ClassDto>> Handle(GetClassOfUserRequest request, CancellationToken cancellationToken)
    {

        var userId = _currentUser.GetUserId();

        var classes = await _repository.ListAsync((ISpecification<Classes, ClassDto>)new ClassByUserSpec(userId), cancellationToken);

        foreach (var classDto in classes)
        {
            var userCount = await _userClassesRepository.GetNumberUserOfClasses(classDto.Id);
            classDto.NumberUserOfClass = userCount;
        }
        return classes;

    }
}
