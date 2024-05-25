using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.New;
public class DeleteNewsReactionRequest : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public Guid NewsId { get; set; }
}

public class DeleteNewsReactionRequestHandler : IRequestHandler<DeleteNewsReactionRequest, Guid>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<News> _newRepository;
    private readonly INewReactionRepository _newReactionRepository;
    private readonly IStringLocalizer _t;

    public DeleteNewsReactionRequestHandler(ICurrentUser currentUser, IRepository<News> newRepository,
                                     INewReactionRepository newReactionRepository, IStringLocalizer<CreateNewsReactionHandler> localizer) =>
        (_currentUser, _newReactionRepository, _newRepository, _t) = (currentUser, newReactionRepository, newRepository, localizer);
    public async Task<DefaultIdType> Handle(DeleteNewsReactionRequest request, CancellationToken cancellationToken)
    {
        var findNews = await _newRepository.GetByIdAsync(request.NewsId, cancellationToken);
        _ = findNews ?? throw new NotFoundException(_t["News {0} Not Found.", request.NewsId]);

        var newsReaction = await _newReactionRepository.GetUserLikeTheNews(new NewsReaction(request.UserId, request.NewsId));

        if (newsReaction is null)
            throw new NotFoundException(_t["User like the News {0} Not Found."]);

        await _newReactionRepository.DeleteNewsReactionAsync(newsReaction);

        return default(DefaultIdType);
    }
}
