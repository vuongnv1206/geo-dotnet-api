using FSH.WebApi.Application.Questions;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.Papers;
public class DeleteQuestionInPaperRequest : IRequest
{
    public Guid PaperId { get; set; }
    public Guid OriginalQuestionId { get; set; }
}

public class DeleteQuestionInPaperRequestHandler : IRequestHandler<DeleteQuestionInPaperRequest>
{
    private readonly IRepository<Paper> _repository;
    private readonly IRepository<QuestionClone> _repositoryQuestionClone;
    private readonly IStringLocalizer<DeleteQuestionInPaperRequestHandler> _t;
    private readonly IMediator _mediator;

    public DeleteQuestionInPaperRequestHandler(IRepository<Paper> repository, IRepository<QuestionClone> repositoryQuestionClone, IStringLocalizer<DeleteQuestionInPaperRequestHandler> t, IMediator mediator)
    {
        _repository = repository;
        _repositoryQuestionClone = repositoryQuestionClone;
        _t = t;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(DeleteQuestionInPaperRequest request, CancellationToken cancellationToken)
    {
        var paper = await _repository.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId), cancellationToken);
        _ = paper
            ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.PaperId]);

        var questionCloneToRemove = paper.PaperQuestions.FirstOrDefault(q => q.Question.OriginalQuestionId == request.OriginalQuestionId);
        paper.RemoveQuestion(questionCloneToRemove.QuestionId);
        await _repository.UpdateAsync(paper, cancellationToken);

        var questionClone = await _repositoryQuestionClone.FirstOrDefaultAsync(new QuestionCloneByIdSpec(questionCloneToRemove.QuestionId));
        if (questionClone == null)
            throw new NotFoundException(_t["Question {0} Not Found.", questionCloneToRemove.QuestionId]);

        await DeleteAllQuestionPassages(questionClone, cancellationToken);
        await _repositoryQuestionClone.DeleteAsync(questionClone);

        return Unit.Value;
    }
    private async Task DeleteAllQuestionPassages(QuestionClone questionClone, CancellationToken cancellationToken)
    {

        // Xóa tất cả các  passages
        foreach (var child in questionClone.QuestionPassages.ToList())
        {
            await DeleteAllQuestionPassages(child, cancellationToken);
            await _repositoryQuestionClone.DeleteAsync(child, cancellationToken);
        }
    }
}
