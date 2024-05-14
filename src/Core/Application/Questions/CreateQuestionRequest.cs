using FSH.WebApi.Domain.Question;
using Mapster;

namespace FSH.WebApi.Application.Questions;
public class CreateQuestionRequest : IRequest<List<Guid>>
{
    public List<CreateQuestionDto> Questions { get; set; }
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

    public async Task<List<DefaultIdType>> Handle(CreateQuestionRequest request, CancellationToken cancellationToken)
    {
        return await AddQuestion(request.Questions);
    }

    private async Task<List<Guid>> AddQuestion(List<CreateQuestionDto> questionDtos)
    {
        var ids = new List<Guid>();
        foreach (var questionRequest in questionDtos)
        {
            var question = questionRequest.Adapt<Question>();

            if (questionRequest.Answers is not null)
            {
                var answers = questionRequest.Answers.Adapt<List<Answer>>();
                question.AddAnswers(answers);
            }

            if (questionRequest.QuestionPassages is not null)
            {
                await AddQuestion(questionRequest.QuestionPassages);
            }

            await _questionRepo.AddAsync(question);
            ids.Add(question.Id);
        }

        return ids;
    }
}
