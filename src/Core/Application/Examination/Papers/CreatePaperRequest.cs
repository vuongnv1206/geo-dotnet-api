﻿using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Questions;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Examination.Enums;
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
    public List<NewQuestionDto>? NewQuestions { get; set; } = new();// Thêm danh sách câu hỏi mới
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
    public CreatePaperRequestHandler(
        IRepositoryWithEvents<Paper> paperRepo,
        IStringLocalizer<CreatePaperRequestHandler> t,
        IRepository<PaperFolder> paperFolderRepo,
        IMediator mediator)
    {
        _paperRepo = paperRepo;
        _t = t;
        _paperFolderRepo = paperFolderRepo;
        _mediator = mediator;
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

        if (!request.Questions.Any() && !request.NewQuestions.Any())
            throw new ConflictException(_t["Create paper must have questions."]);

        if (request.NewQuestions.Any())
        {
            var createQuestionRequest = new CreateQuestionRequest { Questions = request.NewQuestions.Adapt<List<CreateQuestionDto>>() };
            var newQuestionIds = await _mediator.Send(createQuestionRequest, cancellationToken);

            var newPaperQuestions = request.NewQuestions.Select(q => new PaperQuestion
            {
                QuestionId = newQuestionIds[request.NewQuestions.IndexOf(q)],
                Mark = q.Mark,
                RawIndex = q.RawIndex
            }).ToList();
            newPaper.AddQuestions(newPaperQuestions);
        }


        if (request.Questions.Any())
        {
            var questions = request.Questions.Adapt<List<PaperQuestion>>();
            newPaper.AddQuestions(questions);
        }


        await _paperRepo.AddAsync(newPaper);

        return newPaper.Id;
    }
}
