using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;

namespace FSH.WebApi.Application.Questions;

public class SearchQuestionsRequest : PaginationFilter, IRequest<PaginationResponse<QuestionDto>>
{
    public Guid? folderId { get; set; }
    public string? Content { get; set; }
    public QuestionType? QuestionType { get; set; }
    public Guid? QuestionLableId { get; set; }
}

public class SearchQuestionsRequestHandler : IRequestHandler<SearchQuestionsRequest, PaginationResponse<QuestionDto>>
{
    private readonly IReadRepository<Question> _repository;
    private readonly IQuestionService _questionService;
    private readonly IUserService _userService;

    public SearchQuestionsRequestHandler(IReadRepository<Question> repository, IQuestionService questionService, IUserService userService)
    {
        _repository = repository;
        _questionService = questionService;
        _userService = userService;
    }

    public async Task<PaginationResponse<QuestionDto>> Handle(SearchQuestionsRequest request, CancellationToken cancellationToken)
    {
        var folderIds = await _questionService.GetFolderIds(request.folderId!.Value, cancellationToken);

        var spec = new QuestionsBySearchRequestSpec(request, folderIds);
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