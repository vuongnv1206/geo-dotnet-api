using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;

namespace FSH.WebApi.Application.Questions;
public class SearchMyRequestQuestionsRequest : PaginationFilter, IRequest<PaginationResponse<QuestionDto>>
{
    public string? Content { get; set; }
    public QuestionType? QuestionType { get; set; }
    public QuestionStatus? QuestionStatus { get; set; }
}

public class SearchMyRequestQuestionsRequestSpec : EntitiesByPaginationFilterSpec<Question, QuestionDto>
{
    public SearchMyRequestQuestionsRequestSpec(SearchMyRequestQuestionsRequest request, Guid userId, List<Guid> folderIds)
    : base(request) =>
        Query
        .Include(q => q.QuestionPassages)
        .OrderBy(c => c.CreatedOn, !request.HasOrderBy())
        .Where(q => q.CreatedBy.Equals(userId))
        .Where(q => q.QuestionFolderId.HasValue && !folderIds.Contains(q.QuestionFolderId.Value))
        .Where(q => q.Content.Contains(request.Content!), !string.IsNullOrEmpty(request.Content))
        .Where(q => q.QuestionType == request.QuestionType, request.QuestionType.HasValue)
        .Where(q => q.QuestionStatus == request.QuestionStatus, request.QuestionStatus.HasValue);
}

public class SearchMyRequestQuestionsRequestHandler : IRequestHandler<SearchMyRequestQuestionsRequest, PaginationResponse<QuestionDto>>
{
    private readonly IReadRepository<Question> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IQuestionService _questionService;

    public SearchMyRequestQuestionsRequestHandler(IReadRepository<Question> repository, ICurrentUser currentUser, IQuestionService questionService)
    {
        _repository = repository;
        _currentUser = currentUser;
        _questionService = questionService;
    }

    public async Task<PaginationResponse<QuestionDto>> Handle(SearchMyRequestQuestionsRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var folderIds = await _questionService.GetMyFolderIds(userId, cancellationToken);
        var spec = new SearchMyRequestQuestionsRequestSpec(request, userId, folderIds);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}
