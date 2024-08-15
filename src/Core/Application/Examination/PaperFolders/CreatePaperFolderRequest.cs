using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
namespace FSH.WebApi.Application.Examination.PaperFolders;
public class CreatePaperFolderRequest : IRequest<DefaultIdType>
{
    public required string Name { get; set; }
    public DefaultIdType? ParentId { get; set; }
}

public class CreatePaperFolderRequestValidator : CustomValidator<CreatePaperFolderRequest>
{
    public CreatePaperFolderRequestValidator(IReadRepository<PaperFolder> repository, IStringLocalizer<CreatePaperFolderRequestValidator> T) =>
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75)
        .MustAsync(async (request, name, context, ct) =>
        {
            var parentId = request.ParentId;
            return await repository.FirstOrDefaultAsync(new PaperFolderByNameSpec(name, parentId), ct) is null;
        })
            .WithMessage((request, name) => T["PaperFolder {0} already Exists.", name]);
}

public class CreatePaperFolderRequestHandler : IRequestHandler<CreatePaperFolderRequest, DefaultIdType>
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

            if (!parent.CanAdd(_currentUser.GetUserId()))
            {
                throw new ForbiddenException(_t["You do not have permission to create new folder in this folder."]);
            }
        }

        var paperFolder = new PaperFolder(request.Name, request.ParentId);
        await _repository.AddAsync(paperFolder, cancellationToken);

        if (request.ParentId.HasValue)
        {
            var parent = await _repository.FirstOrDefaultAsync(new PaperFolderByIdSpec(request.ParentId.Value), cancellationToken);
            paperFolder.CopyPermissions(parent);

        }
        else //Root
        {
            // add owner permission
            var permission = new PaperFolderPermission(_currentUser.GetUserId(), paperFolder.Id, null, true, true, true, true, true);
            paperFolder.AddPermission(permission);
        }
        await _repository.UpdateAsync(paperFolder, cancellationToken);

        return paperFolder.Id;
    }
}
