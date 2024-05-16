using System.Security.Claims;
using FSH.WebApi.Application.Identity.Roles;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.Question;
using Mapster;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Application.Questions.Specs;
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
    private readonly IRepository<QuestionFolder> _questionFolderRepository;
    private readonly IDapperRepository _repository;
    private readonly IStringLocalizer _t;

    public GetFolderTreeRequestHandler(ICurrentUser currentUser, IUserService userService, IRepository<QuestionFolder> questionFolderRepository, IDapperRepository repository, IStringLocalizer<GetFolderTreeRequestHandler> localizer)
    {
        _currentUser = currentUser;
        _userService = userService;
        _questionFolderRepository = questionFolderRepository;
        _repository = repository;
        _t = localizer;
    }

    public async Task<int> countQuestions(Guid folderId, CancellationToken cancellationToken)
    {
        const string sql = @"
            WITH RECURSIVE RecursiveFolders AS (
                SELECT
                    ""Id"",
                    (SELECT COUNT(*) FROM ""Question"".""Questions"" WHERE ""QuestionFolderId"" = ""QuestionFolders"".""Id"") AS question_count
                FROM ""Question"".""QuestionFolders""
                WHERE ""Id"" = @p0

                UNION ALL

                SELECT
                    qf.""Id"",
                    (SELECT COUNT(*) FROM ""Question"".""Questions"" WHERE ""QuestionFolderId"" = qf.""Id"") AS question_count
                FROM ""Question"".""QuestionFolders"" qf
                INNER JOIN RecursiveFolders rf ON qf.""ParentId"" = rf.""Id""
            )
            SELECT
                SUM(rf.question_count) AS total_questions
            FROM RecursiveFolders rf;
        ";

        return await _repository.RawQuerySingleAsync<int>(sql, new { p0 = folderId }, cancellationToken: cancellationToken);
    }


    public async Task<QuestionTreeDto> Handle(GetFolderTreeRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        var spec = new QuestionFoldersWithPermissionsSpecByUserId(userId, request.ParentId);
        var questionFolders = await _questionFolderRepository.ListAsync(spec, cancellationToken);
        var questionFolderTree = questionFolders.Adapt<List<QuestionTreeDto>>();

        // Get owner details for each folder
        foreach (var tree in questionFolderTree)
        {
            var user = await _userService.GetAsync(tree.CreatedBy.ToString(), cancellationToken);
            if (user != null)
            {
                tree.Owner = user;
            }

            // Get total questions in each folder
            int totalQuestions = await countQuestions(tree.Id, cancellationToken);
            tree.TotalQuestions = totalQuestions;
        }

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
            int totalQuestions = await countQuestions(newTree.Id, cancellationToken);
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

            result = new QuestionTreeDto
            {
                Id = Guid.Empty,
                Name = "Root",
                CurrentShow = false,
                Children = new List<QuestionTreeDto> { newTree }
            };
        }

        return result;
    }

}
