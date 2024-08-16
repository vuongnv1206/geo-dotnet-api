using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Questions;
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

    public CreatePaperRequest(string examName, PaperStatus status, string? password, PaperType type, string? content, string? description, DefaultIdType? paperLabelId, DefaultIdType? paperFolderId, DefaultIdType? subjectId)
    {
        ExamName = examName;
        Status = status;
        Password = password;
        Type = type;
        Content = content;
        Description = description;
        PaperLabelId = paperLabelId;
        PaperFolderId = paperFolderId;
        SubjectId = subjectId;
    }
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
    private readonly ICurrentUser _currentUser;
    public CreatePaperRequestHandler(
        IRepositoryWithEvents<Paper> paperRepo,
        IStringLocalizer<CreatePaperRequestHandler> t,
        IRepository<PaperFolder> paperFolderRepo,
        IMediator mediator,
        IReadRepository<Question> questionRepo,
        IRepository<QuestionClone> questionCloneRepo,
         ICurrentUser currentUser
        )
    {
        _paperRepo = paperRepo;
        _t = t;
        _paperFolderRepo = paperFolderRepo;
        _mediator = mediator;
        _questionRepo = questionRepo;
        _questionCloneRepo = questionCloneRepo;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreatePaperRequest request, CancellationToken cancellationToken)
    {
        if (request.PaperFolderId.HasValue)
        {
            var parent = await _paperFolderRepo.FirstOrDefaultAsync(new PaperFolderByIdSpec(request.PaperFolderId.Value), cancellationToken);
            _ = parent ?? throw new NotFoundException(_t["Folder {0} Not Found.", request.PaperFolderId]);

            if (!parent.CanAdd(_currentUser.GetUserId()))
            {
                throw new ForbiddenException(_t["You do not have permission to create new paper in this folder."]);
            }
        }

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
                newPaper.AddQuestion(paperQuestion);
            }
        }

        await _paperRepo.AddAsync(newPaper);


        // Sao chép quyền từ PaperFolder cha cho paper
        if (request.PaperFolderId.HasValue)
        {
            var parentFolder = await _paperFolderRepo.FirstOrDefaultAsync(new PaperFolderByIdSpec(request.PaperFolderId.Value), cancellationToken);
            if (parentFolder != null)
            {
                foreach (var permission in parentFolder.PaperFolderPermissions)
                {
                    newPaper.AddPermission(new PaperPermission(permission.UserId, newPaper.Id, permission.GroupTeacherId, permission.CanView, permission.CanAdd, permission.CanUpdate, permission.CanDelete, permission.CanShare));
                }
            }
        }
        else //Root
        {
            // add owner permission
            var permission = new PaperPermission(_currentUser.GetUserId(), newPaper.Id, null, true, true, true, true, true);
            newPaper.AddPermission(permission);
        }

        await _paperRepo.UpdateAsync(newPaper);

        return newPaper.Id;
    }
}
