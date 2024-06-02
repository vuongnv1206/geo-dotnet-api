using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Host.Controllers.Question;

public class CreateFolderRequest : IRequest<Guid>
{
    public required string Name { get; set; }
    public Guid? ParentId { get; set; }
}

public class CreateFolderRequestValidator : CustomValidator<CreateFolderRequest>
{
    public CreateFolderRequestValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75);
    }
}

public class CreateFolderRequestHandler : IRequestHandler<CreateFolderRequest, Guid>
{
    private readonly IRepositoryWithEvents<QuestionFolder> _repository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;

    public CreateFolderRequestHandler(IRepositoryWithEvents<QuestionFolder> repository, IStringLocalizer<CreateFolderRequestHandler> localizer, ICurrentUser currentUser)
    {
        _repository = repository;
        _t = localizer;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateFolderRequest request, CancellationToken cancellationToken)
    {
        if (request.ParentId.HasValue)
        {
            var parentFolder = await _repository.FirstOrDefaultAsync(new QuestionFolderByIdSpec(request.ParentId), cancellationToken);
            _ = parentFolder ?? throw new NotFoundException(_t["Folder {0} Not Found.", request.ParentId]);

            if (!parentFolder.CanAdd(_currentUser.GetUserId()))
            {
                throw new ForbiddenException(_t["You do not have permission to create a folder in this folder."]);
            }
        }

        var folder = new QuestionFolder(request.Name, request.ParentId);
        await _repository.AddAsync(folder, cancellationToken);

        if (request.ParentId.HasValue)
        {
            var parentFolder = await _repository.FirstOrDefaultAsync(new QuestionFolderByIdSpec(request.ParentId), cancellationToken);
            folder.CopyPermissions(parentFolder);
        }

        // add owner permission
        bool isHasOwnerPermission = folder.Permissions.Any(x => x.UserId == _currentUser.GetUserId());
        if (!isHasOwnerPermission)
        {
            var permission = new QuestionFolderPermission(_currentUser.GetUserId(), Guid.Empty, folder.Id, true, true, true, true, true);
            folder.AddPermission(permission);
        }

        await _repository.UpdateAsync(folder, cancellationToken);

        return folder.Id;
    }
}