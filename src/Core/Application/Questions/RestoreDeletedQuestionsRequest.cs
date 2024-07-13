namespace FSH.WebApi.Application.Questions;
public class RestoreDeletedQuestionsRequest : IRequest<List<Guid>>
{
    public List<Guid> QuestionIds { get; set; }
}

public class RestoreDeletedQuestionsRequestValidator : AbstractValidator<RestoreDeletedQuestionsRequest>
{
    public RestoreDeletedQuestionsRequestValidator()
    {
        RuleFor(x => x.QuestionIds).NotEmpty();
    }
}

public class RestoreDeletedQuestionsRequestHandler : IRequestHandler<RestoreDeletedQuestionsRequest, List<Guid>>
{
    private readonly IQuestionService _questionService;
    private readonly ICurrentUser _currentUser;

    public RestoreDeletedQuestionsRequestHandler(IQuestionService questionService, ICurrentUser currentUser)
    {
        _questionService = questionService;
        _currentUser = currentUser;
    }

    public async Task<List<Guid>> Handle(RestoreDeletedQuestionsRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        return await _questionService.RestoreDeletedQuestions(userId, request.QuestionIds, cancellationToken);
    }
}
