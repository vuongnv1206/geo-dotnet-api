using FSH.WebApi.Domain.Question;
using Mapster;

namespace FSH.WebApi.Application.Questions;
public class CreateQuestionCloneRequest : IRequest<List<Guid>>
{
    public required List<CreateQuestionDto> Questions { get; set; }
}

public class CreateQuestionCloneRequestValidator : CustomValidator<CreateQuestionCloneRequest>
{
    public CreateQuestionCloneRequestValidator()
    {

    }
}

public class CreateQuestionCloneRequestHandler : IRequestHandler<CreateQuestionCloneRequest, List<Guid>>
{
    private readonly IRepositoryWithEvents<QuestionClone> _questionRepo;
    private readonly IStringLocalizer _t;

    public CreateQuestionCloneRequestHandler(
               IRepositoryWithEvents<QuestionClone> questionRepo,
                      IStringLocalizer<CreateQuestionCloneRequestHandler> t)
    {
        _questionRepo = questionRepo;
        _t = t;
    }

    public async Task<List<Guid>> Handle(CreateQuestionCloneRequest request, CancellationToken cancellationToken)
    {
        var createdQuestionIds = new List<Guid>();
        foreach (var questionDto in request.Questions)
        {
            var question = questionDto.Adapt<QuestionClone>();
            var answers = questionDto.Answers?.Adapt<List<AnswerClone>>();

            if (answers != null)
            {
                question.AddAnswerClones(answers);
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
            passageDto.QuestionParentId = parentId;
            var passage = passageDto.Adapt<QuestionClone>();
            var answers = passageDto.Answers?.Adapt<List<AnswerClone>>();

            if (answers != null)
            {
                passage.AddAnswerClones(answers);
            }

            await _questionRepo.AddAsync(passage, cancellationToken);
            if (passageDto.QuestionPassages != null)
            {
                await AddQuestionPassages(passageDto.QuestionPassages, passage.Id, cancellationToken);
            }
        }
    }
}

