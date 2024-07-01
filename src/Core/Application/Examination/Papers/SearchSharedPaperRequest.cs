
using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Examination.Papers.Specs;
using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.TeacherGroup;
using Mapster;

namespace FSH.WebApi.Application.Examination.Papers;
public class SearchSharedPaperRequest : IRequest<List<PaperInListDto>>
{
    public Guid? PaperFolderId { get; set; }
    public string? Name { get; set; }
}

public class SearchSharedPaperRequestHandler : IRequestHandler<SearchSharedPaperRequest, List<PaperInListDto>>
{
    private readonly IReadRepository<Paper> _repository;
    public readonly IReadRepository<PaperFolder> _paperFolderRepo;
    private readonly IReadRepository<GroupTeacher> _groupTeacherRepo;
    private readonly IReadRepository<PaperPermission> _paperPermissionRepo;

    private readonly ICurrentUser _currentUser;

    public SearchSharedPaperRequestHandler(IReadRepository<Paper> repository, IReadRepository<PaperFolder> paperFolderRepo, IReadRepository<GroupTeacher> groupTeacherRepo, IReadRepository<PaperPermission> paperPermissionRepo, ICurrentUser currentUser)
    {
        _repository = repository;
        _paperFolderRepo = paperFolderRepo;
        _groupTeacherRepo = groupTeacherRepo;
        _paperPermissionRepo = paperPermissionRepo;
        _currentUser = currentUser;
    }

    public async Task<List<PaperInListDto>> Handle(SearchSharedPaperRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();

        // Lấy các nhóm mà người dùng hiện tại thuộc về
        var accessibleGroups = await _groupTeacherRepo.ListAsync(new GroupTeacherByUserSpec(currentUserId), cancellationToken);
        var groupIds = accessibleGroups.Select(g => g.Id).ToList();

        var accessiblePapers = await _paperPermissionRepo.ListAsync(new PaperPermissionByUserOrGroupSpec(currentUserId,groupIds),cancellationToken);
        var accessiblePaperIds = accessiblePapers.Select(p => p.PaperId).Distinct();


        var data = new List<Paper>();

            var spec = new AccessiblePapersSpec(accessiblePaperIds, request);
            data = await _repository.ListAsync(spec, cancellationToken);


        var dtos = new List<PaperInListDto>();
        foreach (var paper in data)
        {
            var dto = paper.Adapt<PaperInListDto>();
            if (paper.PaperFolderId != null)
            {
                var parents = paper.PaperFolder.ListParents();
                dto.Parents = parents.Adapt<List<PaperFolderParentDto>>();
            }
            dtos.Add(dto);
        }
        return dtos;

    }
}