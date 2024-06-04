using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.GroupClasses.Dto;
using FSH.WebApi.Application.Class.UserClasses;
using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FSH.WebApi.Application.Class;
public class SearchClassesRequest : IRequest<List<ClassDto>>
{
    public string? Name { get; set; }
}

public class SearchClassesRequestHandler : IRequestHandler<SearchClassesRequest, List<ClassDto>>
{
    private readonly IReadRepository<Classes> _repository;
    private readonly IStringLocalizer _t;
    private readonly IUserClassesRepository _userClassesRepository;
    private readonly ICurrentUser _currentUser;
    public SearchClassesRequestHandler(IReadRepository<Classes> repository, ICurrentUser currentUser,
                                       IStringLocalizer<SearchClassesRequestHandler> localizer, IUserClassesRepository userClassesRepository)
    {
        _currentUser = currentUser;
        _t = localizer;
        _repository = repository;
        _userClassesRepository = userClassesRepository;
    }

    public async Task<List<ClassDto>> Handle(SearchClassesRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        List<ClassDto> classes;
        if (!string.IsNullOrEmpty(request.Name))
        {
        classes = await _repository.ListAsync((ISpecification<Classes, ClassDto>)new ClassesBySearchRequestWithGroupClassSpec(request.Name, userId), cancellationToken)
        ?? throw new NotFoundException(_t["Classes {0} Not Found.", ""]);
        }
        else{
            classes =  await _repository.ListAsync((ISpecification<Classes, ClassDto>)new ClassByUserSpec(userId), cancellationToken);
        }

        // Count the number of users for each class and update the DTO
        foreach (var classDto in classes)
        {
            var userCount = await _userClassesRepository.GetNumberUserOfClasses(classDto.Id);
            classDto.NumberUserOfClass = userCount;
        }
        return classes;
    }
}
