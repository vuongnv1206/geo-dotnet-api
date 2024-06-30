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

    private readonly IStringLocalizer _t;

    public DeleteNewsRequestHandler(IRepository<Post> repository, IStringLocalizer<DeleteNewsRequestHandler> t)
    {
        _repository = repository;
        _t = t;
    }

    public async Task<DefaultIdType> Handle(DeleteNewsRequest request, CancellationToken cancellationToken)
    {
        var post = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = post ?? throw new NotFoundException(_t["News {0} Not Found."]);

        var postLike = await _repository.ListAsync(new PostLikeByParentIdSpec(request.Id), cancellationToken);
        if (postLike != null)
        {
            await _repository.DeleteRangeAsync(postLike, cancellationToken);
        }

        await _repository.DeleteAsync(post, cancellationToken);

        return request.Id;
    }
}
