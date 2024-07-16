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
    private readonly ICurrentUser _currentUser;

    public UpdateNewsRequestHandler(
        IRepository<Post> repository,
        IStringLocalizer<UpdateNewsRequestHandler> t,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _t = t;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType> Handle(UpdatePostRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var post = await _repository.FirstOrDefaultAsync(new PostByIdSpec(request.Id), cancellationToken)
            ?? throw new NotFoundException(_t["Post {0} Not Found.", request.Id]);

        if (post.CreatedBy != userId)
        {
            throw new ForbiddenException(_t["Post {0} cannot edit", request.Id]);
        }

        var updatePost = post.Update(request.Content, request.IsLockComment);

        post.DomainEvents.Add(EntityUpdatedEvent.WithEntity(post));

        await _repository.UpdateAsync(updatePost, cancellationToken);

        return request.Id;

    }
}
