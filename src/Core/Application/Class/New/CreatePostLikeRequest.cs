﻿using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.New;
public class CreatePostLikeRequest : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public Guid PostsId { get; set; }
}

public class CreateNewsReactionHandler : IRequestHandler<CreatePostLikeRequest, Guid>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<Post> _newRepository;
    private readonly IPostLikeRepository _newReactionRepository;
    private readonly IStringLocalizer _t;

    public CreateNewsReactionHandler(
        ICurrentUser currentUser,
        IRepository<Post> newRepository,
        IPostLikeRepository newReactionRepository,
        IStringLocalizer<CreateNewsReactionHandler> localizer)
    {
        (_currentUser, _newReactionRepository, _newRepository, _t) = (currentUser, newReactionRepository, newRepository, localizer);
    }

    public async Task<DefaultIdType> Handle(CreatePostLikeRequest request, CancellationToken cancellationToken)
    {
        var findPosts = await _newRepository.GetByIdAsync(request.PostsId, cancellationToken);
        _ = findPosts ?? throw new NotFoundException(_t["News {0} Not Found.", request.PostsId]);

        var findUser = _currentUser.GetUserId();
        //if (findUser == Guid.Empty || findUser != request.UserId)
        //{
        //    throw new NotFoundException(_t["User {0} Not Found.", request.UserId]);
        //}

        await _newReactionRepository.AddNewsReaction(new PostLike(request.UserId, request.PostsId));

        return default(DefaultIdType);
    }
}
