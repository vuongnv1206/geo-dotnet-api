namespace FSH.WebApi.Application.Questions;
public class DeleteQuestionsRequest : IRequest<List<Guid>>
{
    public List<Guid> QuestionIds { get; set; }
}

public class DeleteListQuestionsRequestValidator : AbstractValidator<DeleteQuestionsRequest>
{
    public DeleteListQuestionsRequestValidator()
    {
        RuleFor(x => x.QuestionIds).NotEmpty();
    }
}

public class DeleteListQuestionsRequestHandler : IRequestHandler<DeleteQuestionsRequest, List<Guid>>
{
    private readonly IQuestionService _questionService;
    private readonly ICurrentUser _currentUser;

    public DeleteListQuestionsRequestHandler(IQuestionService questionService, ICurrentUser currentUser)
    {
        _questionService = questionService;
        _currentUser = currentUser;
    }

    public async Task<List<Guid>> Handle(DeleteQuestionsRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        return await _questionService.DeleteQuestions(userId, request.QuestionIds, cancellationToken);
    }

}