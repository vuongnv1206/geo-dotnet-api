using FSH.WebApi.Domain.Question;
using Mapster;

namespace FSH.WebApi.Application.Questions;
public class CreateQuestionRequest : IRequest<List<Guid>>
{
    public required List<CreateQuestionDto> Questions { get; set; }
}

public class CreateQuestionRequestValidator : CustomValidator<CreateQuestionRequest>
{
    public CreateQuestionRequestValidator()
    {

    }
}

public class CreateQuestionRequestHandler : IRequestHandler<CreateQuestionRequest, List<Guid>>
{
    private readonly IRepositoryWithEvents<Question> _questionRepo;
    private readonly IStringLocalizer _t;

    public CreateQuestionRequestHandler(
        IRepositoryWithEvents<Question> questionRepo,
        IStringLocalizer<CreateQuestionRequestHandler> t)
    {
        _questionRepo = questionRepo;
        _t = t;
    }

    public async Task<List<Guid>> Handle(CreateQuestionRequest request, CancellationToken cancellationToken)
    {
        var createdQuestionIds = new List<Guid>();
        foreach (var questionDto in request.Questions)
        {
            var question = questionDto.Adapt<Question>();
            var answers = questionDto.Answers?.Adapt<List<Answer>>();

            if (answers != null)
            {
                question.AddAnswers(answers);
            }

            await _questionRepo.AddAsync(question, cancellationToken);
            createdQuestionIds.Add(question.Id);
            if (questionDto.QuestionPassages != null)
            {
                await AddQuestionPassages(questionDto.QuestionPassages, question.Id, cancellationToken);
            }
        }

        return createdQuestionIds;

    }

    private async Task AddQuestionPassages(List<CreateQuestionDto> passages, Guid parentId, CancellationToken cancellationToken)
    {

        foreach (var passageDto in passages)
        {
            passageDto.ParentId = parentId;
            var passage = passageDto.Adapt<Question>();
            var answers = passageDto.Answers?.Adapt<List<Answer>>();

            if (answers != null)
            {
                passage.AddAnswers(answers);
            }

            await _questionRepo.AddAsync(passage, cancellationToken);

            if (passageDto.QuestionPassages != null)
            {
                await AddQuestionPassages(passageDto.QuestionPassages, passage.Id, cancellationToken);
            }
        }
    }

}
