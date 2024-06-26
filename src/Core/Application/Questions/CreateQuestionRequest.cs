using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;
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
        RuleForEach(x => x.Questions).ChildRules(questions =>
        {
            questions.RuleFor(q => q.QuestionType).NotEmpty().WithMessage("Question type is required.");

            questions.When(q => q.QuestionType == QuestionType.MultipleChoice, () =>
            {
                questions.RuleFor(q => q.Answers)
                    .Must(answers => answers != null && answers.Count >= 3)
                    .WithMessage("Multiple choice question must have at least 3 answers.");

                questions.RuleFor(q => q.Answers)
                    .Must(answers => answers != null && answers.Count(a => a.IsCorrect) >= 2)
                    .WithMessage("Multiple choice question must have at least 2 correct answers.");
            });

            questions.When(q => q.QuestionType == QuestionType.SingleChoice, () =>
            {
                questions.RuleFor(q => q.Answers)
                    .Must(answers => answers != null && answers.Count >= 2)
                    .WithMessage("Single choice question must have at least 2 answers.");

                questions.RuleFor(q => q.Answers)
                    .Must(answers => answers != null && answers.Count(a => a.IsCorrect) == 1)
                    .WithMessage("Single choice question must have exactly 1 correct answer.");
            });

            questions.When(q => q.QuestionType == QuestionType.Matching, () =>
            {
                questions.RuleFor(q => q.Answers)
                    .Must(answers => answers != null && answers.Count >= 1)
                    .WithMessage("Matching question must have at least 1 pair.");
                questions.RuleFor(q => q.Answers)
                    .Must(answers => answers != null && answers.All(a => !a.Content.Equals(string.Empty)))
                    .WithMessage("Matching question must have exactly 1 pair.");
            });

            questions.When(q => q.QuestionType == QuestionType.FillBlank, () =>
            {
                questions.RuleFor(q => q.Content)
                    .Must((question, content) => content != null && content.Split("$_fillblank").Length - 1 == question.Answers?.Count)
                    .WithMessage((question, content) => $"Fill in the blank question must have exactly {content.Split("$_fillblank").Length - 1} answers.");
            });

            questions.When(q => q.QuestionType == QuestionType.Writing, () =>
            {
                questions.RuleFor(q => q.Content)
                    .NotEmpty()
                    .WithMessage("Writing question must have content.");
            });

            questions.When(q => q.QuestionType == QuestionType.Reading, () =>
            {
                questions.RuleFor(q => q.QuestionPassages)
                    .NotEmpty()
                    .WithMessage("Reading question must have at least 1 question passage.");

                questions.RuleForEach(q => q.QuestionPassages).ChildRules(passages =>
                {
                    passages.RuleFor(p => p.Answers)
                        .NotEmpty()
                        .WithMessage("Reading question passage must have at least 1 answer.");

                    passages.RuleFor(p => p.Answers)
                        .Must(answers => answers != null && answers.Count >= 2)
                        .WithMessage("Reading question passage must have at least 2 answers.");

                    passages.RuleFor(p => p.Answers)
                        .Must(answers => answers != null && answers.Count(a => a.IsCorrect) >= 1)
                        .WithMessage("Reading question passage must have at least 1 correct answer.");
                });
            });
        });
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
            passageDto.QuestionParentId = parentId;
            var passage = passageDto.Adapt<Question>();
            var answers = passageDto.Answers?.Adapt<List<Answer>>();

            if (answers != null)
            {
                passage.AddAnswers(answers);
            }

            passage.QuestionType = Domain.Question.Enums.QuestionType.ReadingQuestionPassage;
            await _questionRepo.AddAsync(passage, cancellationToken);

            if (passageDto.QuestionPassages != null)
            {
                await AddQuestionPassages(passageDto.QuestionPassages, passage.Id, cancellationToken);
            }
        }
    }
}
