using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions;
public class RejectQuestionsRequest : IRequest<List<Guid>>
{
    public List<Guid> QuestionIds { get; set; } = new();
}

public class RejectQuestionsRequestValidator : AbstractValidator<RejectQuestionsRequest>
{
    public RejectQuestionsRequestValidator()
    {
        RuleFor(x => x.QuestionIds).NotEmpty();
    }
}

public class RejectQuestionsRequestHandler : IRequestHandler<RejectQuestionsRequest, List<Guid>>
{
    private readonly IQuestionService _questionService;
    private readonly ICurrentUser _currentUserService;
    public RejectQuestionsRequestHandler(IQuestionService questionService, ICurrentUser currentUserService)
    {
        _questionService = questionService;
        _currentUserService = currentUserService;
    }

    public async Task<List<Guid>> Handle(RejectQuestionsRequest request, CancellationToken cancellationToken)
    {
        return await _questionService.ChangeQuestionStatus(_currentUserService.GetUserId(), request.QuestionIds, QuestionStatus.Rejected, cancellationToken);
    }
}
