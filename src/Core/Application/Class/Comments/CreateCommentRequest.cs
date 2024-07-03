using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.Comments;
public class CreateCommentRequest : IRequest<Guid>
{
    public Guid PostId { get; set; }
    public string? Content { get; set; }
    public Guid? ParentId { get; set; }
}

public class CreateCommentRequestHandler : IRequestHandler<CreateCommentRequest, Guid>
{
    private readonly IRepository<Comment> _commentRepository;
    private readonly ICurrentUser _currentUser;

    public CreateCommentRequestHandler(IRepository<Comment> commentRepository, ICurrentUser currentUser)
    {
        _commentRepository = commentRepository;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateCommentRequest request, CancellationToken cancellationToken)
    {

        var userId = _currentUser.GetUserId();
        var comment = new Comment(request.PostId, request.Content ?? string.Empty, request.ParentId);

        await _commentRepository.AddAsync(comment);
        return comment.Id;
    }
}
