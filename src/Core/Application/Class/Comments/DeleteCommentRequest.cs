using FSH.WebApi.Application.Class.Comments.Spec;
using FSH.WebApi.Application.Class.New.Spec;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.Comments;
public class DeleteCommentRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public DeleteCommentRequest(Guid id)
    {
        Id = id;
    }
}

public class DeleteCommentRequestHandler : IRequestHandler<DeleteCommentRequest, Guid>
{

    private readonly IRepository<Comment> _repository;
    private readonly IStringLocalizer _t;

    public DeleteCommentRequestHandler(IRepository<Comment> repository, IStringLocalizer<DeleteCommentRequestHandler> t)
    {
        _repository = repository;
        _t = t;
    }

    public async Task<Guid> Handle(DeleteCommentRequest request, CancellationToken cancellationToken)
    {
        var commentTree = await _repository.ListAsync(new CommentTreeSpec(), cancellationToken);
        _ = commentTree ?? throw new NotFoundException(_t["Comment {0} Not Found."]);

        var comment = commentTree.FirstOrDefault(x => x.Id == request.Id);
        _ = comment ?? throw new NotFoundException(_t["Comment {0} Not Found."]);

        await DeleteCommentAndChildren(comment);

        return comment.Id;
    }

    private async Task DeleteCommentAndChildren(Comment comment)
    {
        await _repository.DeleteAsync(comment);

        foreach (var childComment in comment.CommentChildrens)
        {
            await DeleteCommentAndChildren(childComment);
        }
    }
}
