using FSH.WebApi.Domain.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        RuleFor(x => x.Id)
            .NotNull();
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
        var question = await _questionRepo.GetByIdAsync(request.Id);
        _ = question ?? throw new NotFoundException(_t["Question {0} Not Found.", request.Id]);

        if (!question.CanDelete(_currentUser.GetUserId()))
        {
            throw new ForbiddenException(_t["You do not have permission to delete this Question."]);
        }

        await _questionRepo.DeleteAsync(question, cancellationToken);

        return request.Id;
    }
}
