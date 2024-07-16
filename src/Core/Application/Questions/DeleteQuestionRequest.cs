using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions;
public class DeleteQuestionRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public DeleteQuestionRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class DeleteQuestionRequestValidator : CustomValidator<DeleteQuestionRequest>
{
    public DeleteQuestionRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotNull();
    }
}

public class QuestionByIdSpec : Specification<Question>, ISingleResultSpecification
{
    public QuestionByIdSpec(Guid id)
    {
        _ = Query
        .Include(b => b.Answers)
        .Include(b => b.QuestionPassages)
        .Include(b => b.QuestionFolder)
        .ThenInclude(b => b.Permissions)
        .Where(b => b.Id == id);
    }
}

public class DeleteQuestionRequestHandler : IRequestHandler<DeleteQuestionRequest, Guid>
{
    private readonly IRepositoryWithEvents<Question> _questionRepo;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;

    public DeleteQuestionRequestHandler(
        IRepositoryWithEvents<Question> questionRepo,
        IStringLocalizer<DeleteQuestionRequestHandler> t,
        ICurrentUser currentUser)
    {
        _questionRepo = questionRepo;
        _t = t;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(DeleteQuestionRequest request, CancellationToken cancellationToken)
    {
        var question = await _questionRepo.FirstOrDefaultAsync(new QuestionByIdSpec(request.Id), cancellationToken);
        _ = question ?? throw new NotFoundException(_t["Question {0} Not Found.", request.Id]);

        if (!question.CanDelete(_currentUser.GetUserId()))
        {
            // if folder owner is not the current user, throw an exception
            if (!question.QuestionFolder.CreatedBy.Equals(_currentUser.GetUserId()))
            {
                throw new ForbiddenException(_t["You do not have permission to delete this question."]);
            }
        }

        question.DeletedBy = _currentUser.GetUserId();
        question.DeletedOn = DateTime.Now;

        await _questionRepo.UpdateAsync(question, cancellationToken);

        return request.Id;
    }
}
