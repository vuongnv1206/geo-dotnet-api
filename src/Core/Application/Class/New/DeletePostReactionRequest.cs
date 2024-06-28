using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.New;
public class DeletePostReactionRequest : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public Guid NewsId { get; set; }
}

public class DeleteNewsReactionRequestHandler : IRequestHandler<DeletePostReactionRequest, Guid>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<Post> _postRepository;
    private readonly IPostLikeRepository _newReactionRepository;
    private readonly IStringLocalizer _t;

    public DeleteNewsReactionRequestHandler(ICurrentUser currentUser, IRepository<Post> postRepository,
                                     IPostLikeRepository newReactionRepository, IStringLocalizer<CreateNewsReactionHandler> localizer) =>
        (_currentUser, _newReactionRepository, _postRepository, _t) = (currentUser, newReactionRepository, postRepository, localizer);
    public async Task<DefaultIdType> Handle(DeletePostReactionRequest request, CancellationToken cancellationToken)
    {
        var findNews = await _postRepository.GetByIdAsync(request.NewsId, cancellationToken);
        _ = findNews ?? throw new NotFoundException(_t["News {0} Not Found.", request.NewsId]);

        var newsReaction = await _newReactionRepository.GetUserLikeTheNews(new PostLike(request.UserId, request.NewsId));

        if (newsReaction is null)
            throw new NotFoundException(_t["User like the News {0} Not Found."]);

        await _newReactionRepository.DeleteNewsReactionAsync(newsReaction);

        return default(DefaultIdType);
    }
}
