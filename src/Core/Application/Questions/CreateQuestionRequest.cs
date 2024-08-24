using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.Notifications;
using FSH.WebApi.Application.Questions.Specs;
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
    private readonly IStringLocalizer<CreateQuestionRequestValidator> _t;

    public CreateQuestionRequestValidator(IStringLocalizer<CreateQuestionRequestValidator> localizer)
    {
        _t = localizer;

        _ = RuleForEach(x => x.Questions).ChildRules(questions =>
        {
            _ = questions.RuleFor(q => q.QuestionType).NotEmpty().WithMessage(_t["Question Type Required"]);

            _ = questions.When(q => q.QuestionType == QuestionType.MultipleChoice, () =>
            {
                _ = questions.RuleFor(q => q.Answers)
                    .Must(answers => answers != null && answers.Count >= 3)
                    .WithMessage(_t["Multiple Choice At Least 3 Answers"]);

                _ = questions.RuleFor(q => q.Answers)
                    .Must(answers => answers != null && answers.Count(a => a.IsCorrect) >= 2)
                    .WithMessage(_t["Multiple Choice At Least 2 Correct Answers"]);
            });

            _ = questions.When(q => q.QuestionType == QuestionType.SingleChoice, () =>
            {
                _ = questions.RuleFor(q => q.Answers)
                    .Must(answers => answers != null && answers.Count >= 2)
                    .WithMessage(_t["Single Choice At Least 2 Answers"]);

                _ = questions.RuleFor(q => q.Answers)
                    .Must(answers => answers != null && answers.Count(a => a.IsCorrect) == 1)
                    .WithMessage(_t["Single Choice Exactly 1 Correct Answer"]);
            });

            _ = questions.When(q => q.QuestionType == QuestionType.Matching, () =>
            {
                _ = questions.RuleFor(q => q.Answers)
                    .Must(answers => answers != null && answers.Count >= 1)
                    .WithMessage(_t["Matching At Least 1 Pair"]);

                _ = questions.RuleFor(q => q.Answers)
                    .Must(answers => answers != null && answers.All(a => !a.Content.Equals(string.Empty)))
                    .WithMessage(_t["Matching No Empty Pairs"]);
            });

            _ = questions.When(q => q.QuestionType == QuestionType.FillBlank, () =>
            {
                _ = questions.RuleFor(q => q.Content)
                    .Must((question, content) => content != null && content.Split("$_fillblank").Length - 1 == question.Answers?.Count)
                    .WithMessage((question, content) => _t["Fill Blank Exact Answers", content.Split("$_fillblank").Length - 1]);
            });

            _ = questions.When(q => q.QuestionType == QuestionType.Writing, () =>
            {
                _ = questions.RuleFor(q => q.Content)
                    .NotEmpty()
                    .WithMessage(_t["Writing Content Required"]);
            });

            _ = questions.When(q => q.QuestionType == QuestionType.Reading, () =>
            {
                _ = questions.RuleFor(q => q.QuestionPassages)
                    .NotEmpty()
                    .WithMessage(_t["Reading At Least 1 Passage"]);

                _ = questions.RuleForEach(q => q.QuestionPassages).ChildRules(passages =>
                {
                    _ = passages.RuleFor(p => p.Answers)
                        .NotEmpty()
                        .WithMessage(_t["Reading Passage At Least 1 Answer"]);

                    _ = passages.RuleFor(p => p.Answers)
                        .Must(answers => answers != null && answers.Count >= 2)
                        .WithMessage(_t["Reading Passage At Least 2 Answers"]);

                    _ = passages.RuleFor(p => p.Answers)
                        .Must(answers => answers != null && answers.Count(a => a.IsCorrect) >= 1)
                        .WithMessage(_t["Reading Passage At Least 1 Correct Answer"]);
                });
            });
        });
    }
}

public class CreateQuestionRequestHandler : IRequestHandler<CreateQuestionRequest, List<Guid>>
{
    private readonly IRepositoryWithEvents<Question> _questionRepo;
    private readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;
    private readonly IRepositoryWithEvents<QuestionFolder> _questionFolderRepository;
    private readonly IQuestionService _questionService;
    private readonly INotificationService _notificationService;
    private readonly IStringLocalizer<CreateQuestionRequestHandler> _t;

    public CreateQuestionRequestHandler(
        IRepositoryWithEvents<Question> questionRepo,
        ICurrentUser currentUser,
        IUserService userService,
        IRepositoryWithEvents<QuestionFolder> questionFolderRepository,
        IQuestionService questionService,
        INotificationService notificationService,
        IStringLocalizer<CreateQuestionRequestHandler> t)
    {
        _questionRepo = questionRepo;
        _currentUser = currentUser;
        _userService = userService;
        _questionFolderRepository = questionFolderRepository;
        _questionService = questionService;
        _notificationService = notificationService;
        _t = t;
    }

    public async Task<List<Guid>> Handle(CreateQuestionRequest request, CancellationToken cancellationToken)
    {
        var createdQuestionIds = new List<Guid>();
        List<Guid?> questionfolderIds = request.Questions.Select(q => q.QuestionFolderId)
                                .Where(id => id.HasValue).Distinct().ToList();
        List<string> notiUserId = new();

        foreach (var folderId in questionfolderIds)
        {
            var questionFolder = await _questionFolderRepository.FirstOrDefaultAsync(new QuestionFolderByIdSpec(folderId), cancellationToken);
            _ = questionFolder ?? throw new NotFoundException(_t["Folder Not Found."]);

            // check if user has permission to add question to folder
            if (!questionFolder.CanAdd(_currentUser.GetUserId()))
            {
                throw new ForbiddenException(_t["You do not have permission to add question to this folder."]);
            }
        }

        var folderIds = await _questionService.GetFolderIds(questionfolderIds[0].Value, cancellationToken);

        foreach (var questionDto in request.Questions)
        {
            var question = questionDto.Adapt<Question>();
            var answers = questionDto.Answers?.Adapt<List<Answer>>();
            SearchQuestionsRequest searchDuplicate = new()
            {
                Content = questionDto.Content,
                QuestionType = questionDto.QuestionType
            };
            var spec = new QuestionsBySearchEqRequestSpec(searchDuplicate, folderIds);
            var duplicate = await _questionRepo.FirstOrDefaultAsync(spec, cancellationToken);

            if (duplicate != null)
            {
                throw new BadRequestException(_t["There is already a question with the same content and type in the folder."]);
            }

            await SetQuestionStatus(question, questionDto.QuestionFolderId, notiUserId, cancellationToken);
            if (answers != null)
            {
                question.AddAnswers(answers);
            }

            _ = await _questionRepo.AddAsync(question, cancellationToken);
            createdQuestionIds.Add(question.Id);
            if (questionDto.QuestionPassages != null)
            {
                await AddQuestionPassages(questionDto.QuestionPassages, question.Id, cancellationToken);
            }
        }

        string fullName = await _userService.GetFullName(_currentUser.GetUserId());

        var noti = new BasicNotification
        {
            Message = $"{fullName} added new question to your folder.",
            Label = BasicNotification.LabelType.Information,
            Title = "New Question Added",
            Url = "/questions?tab=2"
        };

        await _notificationService.SendNotificationToUsers(notiUserId.Distinct().ToList(), noti, null, cancellationToken);

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

            passage.QuestionType = QuestionType.ReadingQuestionPassage;
            _ = await _questionRepo.AddAsync(passage, cancellationToken);

            if (passageDto.QuestionPassages != null)
            {
                await AddQuestionPassages(passageDto.QuestionPassages, passage.Id, cancellationToken);
            }
        }
    }

    private async Task SetQuestionStatus(Question question, Guid? folderId, List<string> notiUserIds, CancellationToken cancellationToken)
    {
        if (folderId == null)
        {
            return;
        }

        var questionFolder = await _questionService.GetRootFolder(folderId.Value, cancellationToken);

        if (questionFolder.CreatedBy.Equals(_currentUser.GetUserId()))
        {
            question.QuestionStatus = QuestionStatus.Approved;
        }
        else
        {
            question.QuestionStatus = QuestionStatus.Pending;
            notiUserIds.Add(questionFolder.CreatedBy.ToString());
        }
    }
}
