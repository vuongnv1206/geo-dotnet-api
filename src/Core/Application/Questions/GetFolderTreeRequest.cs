using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.TeacherGroup;
using Mapster;
namespace FSH.WebApi.Application.Questions;

public class GetFolderTreeRequest : IRequest<QuestionTreeDto>
{
    public Guid? ParentId { get; }

    public GetFolderTreeRequest(Guid? parentId)
    {
        ParentId = parentId;
    }
}

public class GetFolderTreeRequestHandler : IRequestHandler<GetFolderTreeRequest, QuestionTreeDto>
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;
    private readonly IQuestionService _questionService;
    private readonly IRepository<QuestionFolder> _questionFolderRepository;
    private readonly IRepository<GroupTeacher> _groupTeacherRepository;
    private readonly IStringLocalizer _t;

    public GetFolderTreeRequestHandler(ICurrentUser currentUser, IUserService userService, IQuestionService questionService, IRepository<QuestionFolder> questionFolderRepository, IRepository<GroupTeacher> groupTeacherRepository, IDapperRepository repository, IStringLocalizer<GetFolderTreeRequestHandler> localizer)
    {
        _currentUser = currentUser;
        _userService = userService;
        _questionService = questionService;
        _questionFolderRepository = questionFolderRepository;
        _groupTeacherRepository = groupTeacherRepository;
        _t = localizer;
    }

    public async Task<QuestionTreeDto> Handle(GetFolderTreeRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        var groupTeachers = await _groupTeacherRepository.ListAsync(new GroupTeachersByUserIdSpec(userId), cancellationToken);

        List<Guid> groupTeacherIds = groupTeachers.Select(x => x.Id).ToList();

        var spec = new QuestionFoldersWithPermissionsSpecByUserId(userId, groupTeacherIds, request.ParentId);
        var questionFolders = await _questionFolderRepository.ListAsync(spec, cancellationToken);
        var questionFolderTree = questionFolders.Adapt<List<QuestionTreeDto>>();
        await GetDetails(questionFolderTree, cancellationToken);

        var result = new QuestionTreeDto();

        if (!request.ParentId.HasValue)
        {
            result = new QuestionTreeDto
            {
                Id = Guid.Empty,
                Name = "Root",
                CurrentShow = true,
                Children = questionFolderTree
            };

            // get questions folder if user has no permission to view parent folder but has permission to view child folder
            // get all folders that user has permission to view
            var spec2 = new QuestionChildrenFoldersWithPermissionsSpecByUserId(userId);
            var questionFolders2 = await _questionFolderRepository.ListAsync(spec2, cancellationToken);
            // get folders that user has no permission to view but has permission to view child folder
            foreach (var folder in questionFolders2)
            {
                var parentFolder2 = await _questionFolderRepository.FirstOrDefaultAsync(new QuestionFolderByIdSpec(folder.ParentId.Value), cancellationToken);
                if (parentFolder2 != null && !parentFolder2.Permissions.Any(x => x.UserId == userId && x.CanView))
                {
                    var sharedFolder = folder.Adapt<QuestionTreeDto>();
                    await GetDetails(new List<QuestionTreeDto> { sharedFolder }, cancellationToken);
                    result.Children.Add(sharedFolder);
                }
            }
        }
        else
        {
            // get parent folder
            var parentFolder = await _questionFolderRepository.FirstOrDefaultAsync(new QuestionFolderByIdSpec(request.ParentId.Value), cancellationToken);
            if (parentFolder == null)
            {
                throw new NotFoundException(_t["Folder {0} Not Found.", request.ParentId]);
            }

            var newTree = parentFolder.Adapt<QuestionTreeDto>();

            // Get total questions in each folder
            int totalQuestions = await _questionService.countQuestions(newTree.Id, cancellationToken);
            newTree.TotalQuestions = totalQuestions;
            newTree.Children = questionFolderTree;
            newTree.CurrentShow = true;
            while (newTree.ParentId != null)
            {
                var parent = await _questionFolderRepository.FirstOrDefaultAsync(new QuestionFolderByIdSpec(newTree.ParentId.Value), cancellationToken);
                if (parent == null)
                {
                    throw new NotFoundException(_t["Folder {0} Not Found.", newTree.ParentId]);
                }

                var parentTree = parent.Adapt<QuestionTreeDto>();
                parentTree.Children = new List<QuestionTreeDto> { newTree };
                newTree = parentTree;
            }

            var listNewTree = new List<QuestionTreeDto> { newTree };

            result = new QuestionTreeDto
            {
                Id = Guid.Empty,
                Name = "Root",
                CurrentShow = false,
                Children = listNewTree
            };
        }

        return result;
    }

    private async Task GetDetails(List<QuestionTreeDto> questionFolderTree, CancellationToken cancellationToken)
    {
        // Get owner details for each folder
        foreach (var tree in questionFolderTree)
        {
            var user = await _userService.GetAsync(tree.CreatedBy.ToString(), cancellationToken);
            if (user != null)
            {
                tree.Owner = user;
            }

            // Get total questions in each folder
            int totalQuestions = await _questionService.countQuestions(tree.Id, cancellationToken);
            tree.TotalQuestions = totalQuestions;

            // loop through permissions and set the current user permissions
            foreach (var permission in tree.Permission)
            {
                if (permission.UserId != Guid.Empty)
                {
                    var user_per = await _userService.GetAsync(permission.UserId.ToString(), cancellationToken);
                    if (user_per != null)
                    {
                        permission.User = user_per;
                    }
                }

                if (permission.GroupTeacherId != Guid.Empty)
                {
                    var groupTeacher = await _groupTeacherRepository.FirstOrDefaultAsync(new GroupTeacherByIdSpec(permission.GroupTeacherId), cancellationToken);
                    if (groupTeacher != null)
                    {
                        permission.GroupTeacher = groupTeacher.Adapt<GroupTeacherDto>();
                    }
                }
            }
        }
    }
}
