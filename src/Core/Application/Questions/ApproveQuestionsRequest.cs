using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions;
public class ApproveQuestionsRequest : IRequest<List<Guid>>
{
    public List<Guid> QuestionIds { get; set; } = new();
}

public class ApproveQuestionsRequestValidator : AbstractValidator<ApproveQuestionsRequest>
{
    public ApproveQuestionsRequestValidator()
    {
        RuleFor(x => x.QuestionIds).NotEmpty();
    }
}

public class ApproveQuestionsRequestHandler : IRequestHandler<ApproveQuestionsRequest, List<Guid>>
{
    private readonly IQuestionService _questionService;
    private readonly ICurrentUser _currentUserService;
    public ApproveQuestionsRequestHandler(IQuestionService questionService, ICurrentUser currentUserService)
    {
        _questionService = questionService;
        _currentUserService = currentUserService;
    }

    public async Task<List<Guid>> Handle(ApproveQuestionsRequest request, CancellationToken cancellationToken)
    {
        return await _questionService.ChangeQuestionStatus(_currentUserService.GetUserId(), request.QuestionIds, QuestionStatus.Approved, cancellationToken);
    }
}
