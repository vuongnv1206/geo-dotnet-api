using FSH.WebApi.Application.Class.Comments.Spec;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.Comments;
public class CreateCommentLikeRequest : IRequest<Guid>
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
}

public class CreateCommentLikeHandler : IRequestHandler<CreateCommentLikeRequest, Guid>
{
    private readonly IRepository<Comment> _repository;
    private readonly IStringLocalizer _t;

    public CreateCommentLikeHandler(IRepository<Comment> repository, IStringLocalizer<CreateCommentLikeRequest> t)
    {
        _repository = repository;
        _t = t;
    }

    public async Task<Guid> Handle(CreateCommentLikeRequest request, CancellationToken cancellationToken)
    {
        var comment = await _repository.FirstOrDefaultAsync(new CommentByIdSpec(request.CommentId));

        if(comment is null)
        {
            throw new NotFoundException(_t["Comment {0} Not Found.", request.CommentId]);
        }

        var commentLike = comment.CommentLikes.FirstOrDefault(x => x.UserId == request.UserId);


        if (commentLike is not null)
        {
            throw new BadRequestException(_t["User {0} already like this comment.", request.UserId]);
        }

        comment.AddCommentLike(new CommentLikes
        {
            CommentId = request.CommentId,
            UserId = request.UserId
        });

        await _repository.UpdateAsync(comment);

        return default(Guid);
    }
}   
