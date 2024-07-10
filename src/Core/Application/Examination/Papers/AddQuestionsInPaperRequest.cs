using FSH.WebApi.Application.Questions;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
using Mapster;

namespace FSH.WebApi.Application.Examination.Papers;
public class AddQuestionsInPaperRequest : IRequest
{
    public Guid PaperId { get; set; }
    public List<CreateUpdateQuestionInPaperDto>? Questions { get; set; } // Các câu hỏi hiện có
}


public class AddQuestionsInPaperRequestHandler : IRequestHandler<AddQuestionsInPaperRequest>
{
    private readonly IRepository<Paper> _repository;
    private readonly IRepository<Question> _repositoryQuestion;
    private readonly IStringLocalizer<AddQuestionsInPaperRequestHandler> _t;
    private readonly IMediator _mediator;
    public AddQuestionsInPaperRequestHandler(IRepository<Paper> repository, IRepository<Question> repositoryQuestion, IStringLocalizer<AddQuestionsInPaperRequestHandler> t, IMediator mediator)
    {
        _repository = repository;
        _repositoryQuestion = repositoryQuestion;
        _t = t;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(AddQuestionsInPaperRequest request, CancellationToken cancellationToken)
    {
        var paper = await _repository.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId), cancellationToken);
        _ = paper
            ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.PaperId]);


        foreach (var question in request.Questions)
        {
            var existingQuestion = await _repositoryQuestion.FirstOrDefaultAsync(new Questions.Specs.QuestionByIdSpec(question.QuestionId));
            if (existingQuestion == null)
                throw new NotFoundException(_t["Question {0} Not Found.", question.QuestionId]);

            var createdQuestionCloneId = _mediator.Send(new CreateQuestionCloneRequest
            {
                OriginalQuestionId = question.QuestionId,
            }).Result;

            var paperQuestion = new PaperQuestion
            {
                QuestionId = createdQuestionCloneId,
                Mark = question.Mark,
                RawIndex = question.RawIndex
            };
            paper.AddQuestion(paperQuestion);
        }

        await _repository.UpdateAsync(paper, cancellationToken);

        return Unit.Value;
    }
}