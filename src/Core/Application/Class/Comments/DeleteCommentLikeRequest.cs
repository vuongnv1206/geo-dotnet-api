using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.Comments;
public class DeleteCommentLikeRequest : IRequest<Guid>
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    public DeleteCommentLikeRequest(Guid commentId, DefaultIdType userId)
    {
        CommentId = commentId;
        UserId = userId;
    }
}
public class DeleteCommentLikeRequestHandler : IRequestHandler<DeleteCommentLikeRequest, Guid>
{
    private readonly IStringLocalizer _t;
    private readonly IRepository<Comment> _commentRepository;
    public DeleteCommentLikeRequestHandler(IRepository<Comment> commentRepository
                                           IStringLocalizer<DeleteCommentLikeRequestHandler> stringLocalizer) =>
        (_commentRepository, _t) = (commentRepository, stringLocalizer);

    public async Task<Guid> Handle(DeleteCommentLikeRequest request, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(request.CommentId);
        if (comment == null)
        {
            throw new NotFoundException(_t["Comment {0} Not Found.", request.CommentId]);
        }

        if (comment.CommentLikes.Any())
        {
            var commentlike = comment.CommentLikes?
                .FirstOrDefault(x => x.UserId == request.UserId);

            if (commentlike is null)
                throw new NotFoundException(_t["Student {0} Not Found.", request.UserId]);

            comment.RemoveCommentLike(commentlike);

            await _commentRepository.UpdateAsync(comment);
        }
        return default(Guid);
    }
}
