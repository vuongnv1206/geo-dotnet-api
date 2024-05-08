using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Host.Controllers.Question;

public class UpdateFolderRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid? ParentId { get; set; }
}

public class UpdateFolderRequestValidator : CustomValidator<UpdateFolderRequest>
{
    public UpdateFolderRequestValidator(IReadRepository<QuestionFolder> repository, IStringLocalizer<UpdateFolderRequestValidator> T)
    {
        RuleFor(p => p.ParentId)
            .MustAsync(async (parentId, ct) => parentId is null || await repository.FirstOrDefaultAsync(new QuestionFolderByIdSpec(parentId), ct) is not null)
                .WithMessage((_, parentId) => T["Parent folder {0} does not exist.", parentId]);
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75);
        RuleFor(p => p.Id)
            .NotEmpty();
    }
}

public class UpdateFolderRequestHandler : IRequestHandler<UpdateFolderRequest, Guid>
{
    private readonly IRepositoryWithEvents<QuestionFolder> _repository;
    private readonly IStringLocalizer _t;

    public UpdateFolderRequestHandler(IRepositoryWithEvents<QuestionFolder> repository, IStringLocalizer<UpdateFolderRequestHandler> localizer)
    {
        _repository = repository;
        _t = localizer;
    }

    public async Task<Guid> Handle(UpdateFolderRequest request, CancellationToken cancellationToken)
    {
        var folder = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = folder ?? throw new NotFoundException(_t["Folder {0} Not Found.", request.Id]);

        var updatedFolder = folder.Update(request.Name, request.ParentId);

        await _repository.UpdateAsync(updatedFolder, cancellationToken);

        return folder.Id;
    }
}