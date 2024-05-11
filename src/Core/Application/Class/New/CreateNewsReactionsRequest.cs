using FSH.WebApi.Application.Class.GroupClasses;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New;
public class CreateNewsReactionsRequest : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public Guid NewsId { get; set; }
}

public class CreateNewsReactionHandler : IRequestHandler<CreateNewsReactionsRequest, Guid>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<News> _newRepository;
    private readonly INewReactionRepository _newReactionRepository;
    private readonly IStringLocalizer _t;

    public CreateNewsReactionHandler(ICurrentUser currentUser, IRepository<News> newRepository,
                                     INewReactionRepository newReactionRepository, IStringLocalizer<CreateNewsReactionHandler> localizer) =>
        (_currentUser, _newReactionRepository , _newRepository, _t) = (currentUser, newReactionRepository,newRepository, localizer);
    public async Task<DefaultIdType> Handle(CreateNewsReactionsRequest request, CancellationToken cancellationToken)
    {
        var findNews = await _newRepository.GetByIdAsync(request.NewsId, cancellationToken);
        _ = findNews ?? throw new NotFoundException(_t["News {0} Not Found.", request.NewsId]);

        var findUser = _currentUser.GetUserId();
        if (findUser == null || findUser != request.UserId)
        {
            throw new NotFoundException(_t["User {0} Not Found.", request.UserId]);
        }

        await _newReactionRepository.AddNewsReaction(new NewsReaction(request.UserId, request.NewsId));

        return default(DefaultIdType);
    }
}