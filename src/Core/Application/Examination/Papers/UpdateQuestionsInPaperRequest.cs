
using FSH.WebApi.Application.Questions;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
using Mapster;

namespace FSH.WebApi.Application.Examination.Papers;
public class UpdateQuestionsInPaperRequest : IRequest
{
    public Guid PaperId { get; set; }
    public List<CreateUpdateQuestionInPaperDto> Questions { get; set; }
}


public class UpdateQuestionsInPaperRequestHandler : IRequestHandler<UpdateQuestionsInPaperRequest>
{
    private readonly IRepository<Paper> _repository;
    private readonly IRepository<Question> _repositoryQuestion;
    private readonly IStringLocalizer<UpdateQuestionsInPaperRequestHandler> _t;

    public UpdateQuestionsInPaperRequestHandler(IRepository<Paper> repository, IRepository<Question> repositoryQuestion, IStringLocalizer<UpdateQuestionsInPaperRequestHandler> t)
    {
        _repository = repository;
        _repositoryQuestion = repositoryQuestion;
        _t = t;
    }

    public async Task<Unit> Handle(UpdateQuestionsInPaperRequest request, CancellationToken cancellationToken)
    {
        var paper = await _repository.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId), cancellationToken);
        _ = paper
            ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.PaperId]);


        var questionIds = request.Questions.Select(q => q.QuestionId).ToList();
        var questions = await _repositoryQuestion.ListAsync(new QuestionsByIdsSpec(questionIds), cancellationToken);

        var paperQuestions = request.Questions.Adapt<List<PaperQuestion>>();

        paper.UpdateQuestions(paperQuestions);

        await _repository.UpdateAsync(paper, cancellationToken);

        return Unit.Value;
    }
}
