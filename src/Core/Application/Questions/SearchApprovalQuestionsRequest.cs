using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;

namespace FSH.WebApi.Application.Questions;
public class SearchApprovalQuestionsRequest : PaginationFilter, IRequest<PaginationResponse<QuestionDto>>
{
    public string? Content { get; set; }
    public QuestionType? QuestionType { get; set; }
}

public class SearchApprovalQuestionsRequestSpec : EntitiesByPaginationFilterSpec<Question, QuestionDto>
{
    public SearchApprovalQuestionsRequestSpec(SearchApprovalQuestionsRequest request, Guid userId, List<Guid> folderIds)
    : base(request) =>
        Query
        .Include(q => q.QuestionPassages)
        .OrderBy(c => c.CreatedOn, !request.HasOrderBy())
        .Where(q => q.QuestionStatus == QuestionStatus.Pending)
        .Where(q => q.QuestionFolderId.HasValue && folderIds.Contains(q.QuestionFolderId.Value))
        .Where(q => q.Content.Contains(request.Content!), !string.IsNullOrEmpty(request.Content))
        .Where(q => q.QuestionType == request.QuestionType, request.QuestionType.HasValue);
}

public class SearchApprovalQuestionsRequestHandler : IRequestHandler<SearchApprovalQuestionsRequest, PaginationResponse<QuestionDto>>
{
    private readonly IReadRepository<Question> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IQuestionService _questionService;
    private readonly IUserService _userService;

    public SearchApprovalQuestionsRequestHandler(IReadRepository<Question> repository, ICurrentUser currentUser, IQuestionService questionService, IUserService userService)
    {
        _repository = repository;
        _currentUser = currentUser;
        _questionService = questionService;
        _userService = userService;
    }

    public async Task<PaginationResponse<QuestionDto>> Handle(SearchApprovalQuestionsRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var folderIds = await _questionService.GetMyFolderIds(userId, cancellationToken);
        var spec = new SearchApprovalQuestionsRequestSpec(request, userId, folderIds);
        var res = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);

        // Get owner details for each Question
        foreach (var question in res.Data)
        {
            try
            {
                var user = await _userService.GetAsync(question.CreatedBy.ToString(), cancellationToken);
                if (user != null)
                {
                    question.Owner = user;
                }
            }
            catch
            {

            }
        }

        return res;
    }
}
