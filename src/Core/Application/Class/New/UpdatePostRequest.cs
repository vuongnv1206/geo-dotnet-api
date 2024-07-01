using FSH.WebApi.Application.Class.New.Spec;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Common.Events;

namespace FSH.WebApi.Application.Class.New;
public class UpdatePostRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public required string Content { get; set; }
    public bool IsLockComment { get; set; }
}

public class UpdateNewsRequestHandler : IRequestHandler<UpdatePostRequest, Guid>
{

    public readonly IRepository<Post> _repository;
    private readonly IStringLocalizer _t;

    public UpdateNewsRequestHandler(IRepository<Post> repository, IStringLocalizer<UpdateNewsRequestHandler> localizer) =>
        (_repository, _t) = (repository, localizer);

    public async Task<DefaultIdType> Handle(UpdatePostRequest request, CancellationToken cancellationToken)
    {
        //var post = await _repository.GetByIdAsync(request.Id, cancellationToken);

        var post = await _repository.FirstOrDefaultAsync(new PostByIdSpec(request.Id), cancellationToken);
        _ = post ?? throw new NotFoundException(_t["Post {0} Not Found.", request.Id]);

        var updatePost = post.Update(request.Content, request.IsLockComment);

        post.DomainEvents.Add(EntityUpdatedEvent.WithEntity(post));

        await _repository.UpdateAsync(updatePost, cancellationToken);

        return request.Id;

    }
}
