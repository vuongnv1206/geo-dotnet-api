using FSH.WebApi.Application.Questions;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
using Mapster;

namespace FSH.WebApi.Application.Examination.Papers;
public class UpdateQuestionsInPaperRequest : IRequest
{
    public Guid PaperId { get; set; }
    public List<CreateUpdateQuestionInPaperDto> Questions { get; set; } // Các câu hỏi hiện có
    public List<NewQuestionDto> NewQuestions { get; set; } // Các câu hỏi mới
}


public class UpdateQuestionsInPaperRequestHandler : IRequestHandler<UpdateQuestionsInPaperRequest>
{
    private readonly IRepository<Paper> _repository;
    private readonly IRepository<Question> _repositoryQuestion;
    private readonly IStringLocalizer<UpdateQuestionsInPaperRequestHandler> _t;
    private readonly IMediator _mediator;
    public UpdateQuestionsInPaperRequestHandler(IRepository<Paper> repository, IRepository<Question> repositoryQuestion, IStringLocalizer<UpdateQuestionsInPaperRequestHandler> t, IMediator mediator)
    {
        _repository = repository;
        _repositoryQuestion = repositoryQuestion;
        _t = t;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(UpdateQuestionsInPaperRequest request, CancellationToken cancellationToken)
    {
        var paper = await _repository.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId), cancellationToken);
        _ = paper
            ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.PaperId]);

        if (!request.Questions.Any() && !request.NewQuestions.Any())
            throw new ConflictException(_t["Paper must have questions."]);

        if (request.NewQuestions.Any())
        {
            var createQuestionRequest = new CreateQuestionRequest { Questions = request.NewQuestions.Adapt<List<CreateQuestionDto>>() };
            var newQuestionIds = await _mediator.Send(createQuestionRequest, cancellationToken);
            var newPaperQuestions = newQuestionIds.Select((id, index) => new PaperQuestion
            {
                QuestionId = id,
                PaperId = paper.Id,
                Mark = request.NewQuestions[index].Mark
            }).ToList();

            request.Questions.AddRange(newPaperQuestions.Select(x => new CreateUpdateQuestionInPaperDto
            {
                QuestionId = x.QuestionId,
                Mark = x.Mark
            }));
        }

        var questionIds = request.Questions.Select(q => q.QuestionId).ToList();
        var questions = await _repositoryQuestion.ListAsync(new QuestionsByIdsSpec(questionIds), cancellationToken);

        var paperQuestions = request.Questions.Adapt<List<PaperQuestion>>();

        paper.UpdateQuestions(paperQuestions);

        await _repository.UpdateAsync(paper, cancellationToken);

        return Unit.Value;
    }
}