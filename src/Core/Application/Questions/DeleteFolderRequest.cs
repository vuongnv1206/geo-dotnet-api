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

    public DeleteFolderRequestHandler(IRepositoryWithEvents<QuestionFolder> repository, IStringLocalizer<DeleteFolderRequestHandler> localizer)
    {
        _repository = repository;
        _t = localizer;
    }

    public async Task<DefaultIdType> Handle(DeleteFolderRequest request, CancellationToken cancellationToken)
    {
        var folder = await _repository.GetByIdAsync(request.Id, cancellationToken);
        _ = folder ?? throw new NotFoundException(_t["Folder {0} Not Found.", request.Id]);

        await _repository.DeleteAsync(folder, cancellationToken);

        return folder.Id;
    }
}