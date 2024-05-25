using FSH.WebApi.Application.Questions;
using FSH.WebApi.Domain.Question;
using MediatR;

namespace FSH.WebApi.Host.Controllers.Question;
public class DeleteFolderRequest : IRequest<Guid>
{
    public Guid Id { get; set; }

    public DeleteFolderRequest(Guid id)
    {
        Id = id;
    }
}

public class DeleteFolderRequestValidator : AbstractValidator<DeleteFolderRequest>
{
    public DeleteFolderRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class DeleteFolderRequestHandler : IRequestHandler<DeleteFolderRequest, Guid>
{
    private readonly IRepositoryWithEvents<QuestionFolder> _repository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;

    public DeleteFolderRequestHandler(IRepositoryWithEvents<QuestionFolder> repository, IStringLocalizer<DeleteFolderRequestHandler> localizer, ICurrentUser currentUser)
    {
        _repository = repository;
        _t = localizer;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType> Handle(DeleteFolderRequest request, CancellationToken cancellationToken)
    {
        var folder = await _repository.FirstOrDefaultAsync(new QuestionFolderByIdSpec(request.Id), cancellationToken);
        _ = folder ?? throw new NotFoundException(_t["Folder {0} Not Found.", request.Id]);

        if (!folder.CanDelete(_currentUser.GetUserId()))
        {
            throw new ForbiddenException(_t["You do not have permission to delete this folder."]);
        }

        await _repository.DeleteAsync(folder, cancellationToken);

        return folder.Id;
    }
}