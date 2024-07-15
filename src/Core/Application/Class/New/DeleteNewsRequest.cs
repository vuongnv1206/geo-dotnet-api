using FSH.WebApi.Application.Class.New.Spec;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.New;
public class DeleteNewsRequest : IRequest<Guid>
{
    public Guid Id { get; set; }

    public DeleteNewsRequest(Guid id) => Id = id;
}

public class DeleteNewsRequestHandler : IRequestHandler<DeleteNewsRequest, Guid>
{
    private readonly IRepository<Post> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;

    public DeleteNewsRequestHandler(
        IRepository<Post> repository,
        IStringLocalizer<DeleteNewsRequestHandler> t,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _t = t;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType> Handle(DeleteNewsRequest request, CancellationToken cancellationToken)
    {
        var post = await _repository.FirstOrDefaultAsync(new PostByIdSpec(request.Id), cancellationToken)
            ?? throw new NotFoundException(_t["News {0} Not Found."]);

        if (post.CreatedBy != _currentUser.GetUserId())
            throw new ForbiddenException(_t["Post {0} cannot delete", request.Id]);

        await _repository.DeleteAsync(post, cancellationToken);

        return request.Id;
    }
}
