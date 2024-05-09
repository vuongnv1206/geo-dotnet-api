using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Domain.Examination;
namespace FSH.WebApi.Application.Examination;
public class CreatePaperFolderRequest : IRequest<Guid>
{
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? SubjectId { get; set; }
}

public class CreatePaperFolderRequestValidator : CustomValidator<CreatePaperFolderRequest>
{
    public CreatePaperFolderRequestValidator(IReadRepository<PaperFolder> repository, IStringLocalizer<CreatePaperFolderRequestValidator> T) =>
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (name, ct) => await repository.FirstOrDefaultAsync(new PaperFolderByNameSpec(name), ct) is null)
                .WithMessage((_, name) => T["PaperFolder {0} already Exists.", name]);
}

public class CreatePaperFolderRequestHandler : IRequestHandler<CreatePaperFolderRequest, Guid>
{
    private readonly IRepository<PaperFolder> _repository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;
    public CreatePaperFolderRequestHandler(IRepository<PaperFolder> repository, IStringLocalizer<CreatePaperFolderRequestHandler> t, ICurrentUser currentUser)
    {
        _repository = repository;
        _t = t;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType> Handle(CreatePaperFolderRequest request, CancellationToken cancellationToken)
    {

        if (request.ParentId.HasValue)
        {
            var parent = await _repository.FirstOrDefaultAsync(new PaperFolderByIdSpec(request.ParentId.Value), cancellationToken);
            _ = parent ?? throw new NotFoundException(_t["Folder {0} Not Found.", request.ParentId]);
        }

        var paperFolder = new PaperFolder(request.Name, request.ParentId, request.SubjectId);
        await _repository.AddAsync(paperFolder, cancellationToken);

        return paperFolder.Id;
    }
}
