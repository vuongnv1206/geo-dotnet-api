using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;

namespace FSH.WebApi.Application.Questions;
public class SearchMyDeletedQuestions : PaginationFilter, IRequest<PaginationResponse<QuestionDto>>
{
    public string? Content { get; set; }
    public QuestionType? QuestionType { get; set; }
}

public class SearchMyDeletedQuestionsSpec : EntitiesByPaginationFilterSpec<Question, QuestionDto>
{
    public SearchMyDeletedQuestionsSpec(SearchMyDeletedQuestions request, Guid userId)
    : base(request) =>
        Query
        .IgnoreQueryFilters()
        .Include(q => q.QuestionPassages)
        .OrderByDescending(c => c.DeletedOn)
        .Where(q => q.QuestionFolderId.HasValue)
        .Where(q => q.DeletedBy == userId)
        .Where(q => q.Content.Contains(request.Content!), !string.IsNullOrEmpty(request.Content))
        .Where(q => q.QuestionType == request.QuestionType, request.QuestionType.HasValue);
}

public class SearchMyDeletedQuestionsHandler : IRequestHandler<SearchMyDeletedQuestions, PaginationResponse<QuestionDto>>
{
    private readonly IReadRepository<Question> _repository;
    private readonly ICurrentUser _currentUser;

    public SearchMyDeletedQuestionsHandler(IReadRepository<Question> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PaginationResponse<QuestionDto>> Handle(SearchMyDeletedQuestions request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var spec = new SearchMyDeletedQuestionsSpec(request, userId);
        var res = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);

        return res;
    }
}
