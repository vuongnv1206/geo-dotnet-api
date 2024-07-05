
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
    private readonly IReadRepository<PaperFolderPermission> _paperFolderPermissionRepo;

    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;
    

    public SearchSharedPaperRequestHandler(IReadRepository<Paper> repository,
        IReadRepository<PaperFolder> paperFolderRepo, IReadRepository<GroupTeacher>
        groupTeacherRepo, IReadRepository<PaperPermission> paperPermissionRepo,
        ICurrentUser currentUser, IReadRepository<PaperFolderPermission> paperFolderPermissionRepo,
        IStringLocalizer<SearchSharedPaperRequestHandler> t)
    {
        _repository = repository;
        _paperFolderRepo = paperFolderRepo;
        _groupTeacherRepo = groupTeacherRepo;
        _paperPermissionRepo = paperPermissionRepo;
        _currentUser = currentUser;
        _paperFolderPermissionRepo = paperFolderPermissionRepo;
        _t = t;

    }

    public async Task<List<PaperInListDto>> Handle(SearchSharedPaperRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();

        // Lấy các nhóm mà người dùng hiện tại thuộc về
        var accessibleGroups = await _groupTeacherRepo.ListAsync(new GroupTeacherByUserSpec(currentUserId), cancellationToken);
        var groupIds = accessibleGroups.Select(g => g.Id).ToList();

        var accessiblePapers = await _paperPermissionRepo.ListAsync(new PaperPermissionByUserOrGroupSpec(currentUserId,groupIds),cancellationToken);
        var accessiblePaperIds = accessiblePapers.Select(p => p.PaperId).Distinct();

        var accessibleFolders = await _paperFolderPermissionRepo.ListAsync(new PaperFolderPermissionByUserOrGroupSpec(currentUserId, groupIds), cancellationToken);
        var accessibleFolderIds = accessibleFolders.Select(p => p.FolderId).Distinct();

        var data = new List<Paper>();

        //If Search by Name
        if (!string.IsNullOrEmpty(request.Name))
        {
            if (request.PaperFolderId.HasValue)  //Folder
            {
                var folderTree = await _paperFolderRepo.ListAsync(new PaperFolderTreeSpec(), cancellationToken);
                var folderParent = folderTree.FirstOrDefault(x => x.Id == request.PaperFolderId)
                 ?? throw new NotFoundException(_t["The Folder {0} Not Found", request.PaperFolderId]);

                var folderChildrenIds = new List<Guid>();
                folderParent.ChildPaperFolderIds(null, folderChildrenIds);

                var searchableFolderIds = accessibleFolderIds.Intersect(folderChildrenIds);
                var spec = new AccessiblePaperInSearchSpec(accessiblePaperIds, searchableFolderIds, request);
                data.AddRange(await _repository.ListAsync(spec, cancellationToken));
            }
            else    //Root
            {
                var specRoot = new AccessiblePaperInSearchSpec(accessiblePaperIds, accessibleFolderIds, request);
                data.AddRange(await _repository.ListAsync(specRoot, cancellationToken));
            }

        }
        else
        {
            //If Root
            if (!request.PaperFolderId.HasValue)
            {
                //trường hợp paper không thuộc folder nào == root

                var specRoot = new AccessiblePapersSpec(accessiblePaperIds, request);
                data.AddRange(await _repository.ListAsync(specRoot, cancellationToken));
                //trường hợp paper thuộc folder nhưng folder đó không được share
                var specChild = new SharedPapersInChildFolderSpec(accessiblePaperIds, accessibleFolderIds, request);
                data.AddRange(await _repository.ListAsync(specChild, cancellationToken));
            }
            else //If not Root (Folder)
            {
                //lấy những paper được share trong folder đó
                var specRoot = new AccessiblePapersSpec(accessiblePaperIds, request);
                data.AddRange(await _repository.ListAsync(specRoot, cancellationToken));
            }
        }

     

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