using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Examination.Enums;
using Mapster;

namespace FSH.WebApi.Application.Examination.Papers;
public class CreatePaperRequest : IRequest<PaperDto>
{
    public string ExamName { get; set; } = null!;
    public PaperStatus Status { get; set; }
    public string? Password { get; set; }
    public PaperType Type { get; set; }
    public Guid? PaperFolderId { get; set; }
    public string? Content { get; set; }
    public string? Description { get; set; }
    public Dictionary<Guid, float> Questions { get; set; }
}

public class CreatePaperRequestValidator : CustomValidator<CreatePaperRequest>
{
    public CreatePaperRequestValidator()
    {
        RuleFor(x => x.ExamName)
            .NotEmpty();
    }
}

public class CreatePaperRequestHandler : IRequestHandler<CreatePaperRequest, PaperDto>
{
    private readonly IRepositoryWithEvents<Paper> _paperRepo;
    private readonly IStringLocalizer<CreatePaperRequestHandler> _t;
    private readonly IRepository<PaperFolder> _paperFolderRepo;

    public CreatePaperRequestHandler(
        IRepositoryWithEvents<Paper> paperRepo,
        IStringLocalizer<CreatePaperRequestHandler> t,
        IRepository<PaperFolder> paperFolderRepo)
    {
        _paperRepo = paperRepo;
        _t = t;
        _paperFolderRepo = paperFolderRepo;
    }

    public async Task<PaperDto> Handle(CreatePaperRequest request, CancellationToken cancellationToken)
    {
        if (request.PaperFolderId.HasValue
            && await _paperFolderRepo.AnyAsync(new PaperFolderByIdSpec(request.PaperFolderId.Value)))
            throw new NotFoundException(_t["Paper folder {0} Not Found.", request.PaperFolderId]);

        var newPaper = new Paper(
            request.ExamName,
            request.Status,
            request.Type,
            request.Content,
            request.Description,
            request.PaperFolderId,
            request.Password);

        if (!request.Questions.Any())
            throw new ConflictException(_t["Create paper must to have question"]);

        newPaper.AddQuestions(request.Questions);

        await _paperRepo.AddAsync( newPaper );
        var paperDto = newPaper.Adapt<PaperDto>();

        return paperDto;
    }
}
