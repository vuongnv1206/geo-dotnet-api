using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Examination.Enums;
using FSH.WebApi.Domain.Question;
using Mapster;

namespace FSH.WebApi.Application.Examination.Papers;
public class CreatePaperRequest : IRequest<Guid>
{
    public string ExamName { get; set; } = null!;
    public PaperStatus Status { get; set; }
    public string? Password { get; set; }
    public PaperType Type { get; set; }
    public string? Content { get; set; }
    public string? Description { get; set; }
    public Guid? PaperLabelId { get; set; }
    public Guid? PaperFolderId { get; set; }
    public Guid? SubjectId { get; set; }
    public List<CreateUpdateQuestionInPaperDto>? Questions { get; set; } = new(); // Thêm danh sách câu hỏi đã có
}

public class CreatePaperRequestValidator : CustomValidator<CreatePaperRequest>
{
    public CreatePaperRequestValidator()
    {
        RuleFor(x => x.ExamName)
            .NotEmpty();
    }
}

public class CreatePaperRequestHandler : IRequestHandler<CreatePaperRequest, Guid>
{
    private readonly IRepositoryWithEvents<Paper> _paperRepo;
    private readonly IStringLocalizer<CreatePaperRequestHandler> _t;
    private readonly IRepository<PaperFolder> _paperFolderRepo;
    private readonly IMediator _mediator;
    private readonly IReadRepository<Question> _questionRepo;
    private readonly IRepository<QuestionClone> _questionCloneRepo;
    public CreatePaperRequestHandler(
        IRepositoryWithEvents<Paper> paperRepo,
        IStringLocalizer<CreatePaperRequestHandler> t,
        IRepository<PaperFolder> paperFolderRepo,
        IMediator mediator,
        IReadRepository<Question> questionRepo,
        IRepository<QuestionClone> questionCloneRepo
        )
    {
        _paperRepo = paperRepo;
        _t = t;
        _paperFolderRepo = paperFolderRepo;
        _mediator = mediator;
        _questionRepo = questionRepo;
        _questionCloneRepo = questionCloneRepo;
    }

    public async Task<Guid> Handle(CreatePaperRequest request, CancellationToken cancellationToken)
    {
        if (request.PaperFolderId.HasValue
            && !await _paperFolderRepo.AnyAsync(new PaperFolderByIdSpec(request.PaperFolderId.Value)))
            throw new NotFoundException(_t["Paper folder {0} Not Found.", request.PaperFolderId]);

        var newPaper = new Paper(
            request.ExamName,
            request.Status,
            request.Type,
            request.Content,
            request.Description,
            request.Password,
            request.PaperFolderId,
            request.PaperLabelId,
            request.SubjectId);

        if (!request.Questions.Any())
            throw new ConflictException(_t["Create paper must have questions."]);

        if (request.Questions.Any())
        {
            foreach (var question in request.Questions)
            {
                var existingQuestion = await _questionRepo.FirstOrDefaultAsync(new Questions.Specs.QuestionByIdSpec(question.QuestionId));
                if (existingQuestion == null)
                    throw new NotFoundException(_t["Question {0} Not Found.", question.QuestionId]);

                var questionClone = existingQuestion.Adapt<QuestionClone>();
                questionClone.OriginalQuestionId = existingQuestion.Id;
                await _questionCloneRepo.AddAsync(questionClone);

                var paperQuestion = new PaperQuestion
                {
                    QuestionId = questionClone.Id,
                    Mark = question.Mark,
                    RawIndex = question.RawIndex
                };
                newPaper.AddQuestion(paperQuestion);
            }
        }


        await _paperRepo.AddAsync(newPaper);

        return newPaper.Id;
    }
}
