using FSH.WebApi.Domain.Question;
using FSH.WebApi.Application.Questions.Specs;

namespace FSH.WebApi.Host.Controllers.Question;

public class CreateFolderRequest : IRequest<Guid>
{
    public required string Name { get; set; }
    public Guid? ParentId { get; set; }
}

public class CreateFolderRequestValidator : CustomValidator<CreateFolderRequest>
{
    public CreateFolderRequestValidator(IReadRepository<QuestionFolder> repository, IStringLocalizer<CreateFolderRequestValidator> T)
    {
        RuleFor(p => p.ParentId)
            .MustAsync(async (parentId, ct) => parentId is null || await repository.FirstOrDefaultAsync(new QuestionFolderByIdSpec(parentId), ct) is not null)
                .WithMessage((_, parentId) => T["Parent folder {0} does not exist.", parentId]);
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75);
    }
}

public class CreateFolderRequestHandler : IRequestHandler<CreateFolderRequest, Guid>
{
    private readonly IRepositoryWithEvents<QuestionFolder> _repository;

    public CreateFolderRequestHandler(IRepositoryWithEvents<QuestionFolder> repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateFolderRequest request, CancellationToken cancellationToken)
    {
        var folder = new QuestionFolder(request.Name, request.ParentId);
        await _repository.AddAsync(folder, cancellationToken);

        return folder.Id;
    }
}