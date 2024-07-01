using FSH.WebApi.Application.Class.GroupClasses;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.Comments;
public class UpdateCommentRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string? Content { get; set; }
    public Guid? ParentId { get; set; }
}

public class UpdateCommentRequestHandler : IRequestHandler<UpdateCommentRequest, Guid>
{
    private readonly IRepositoryWithEvents<Comment> _repository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;
    public UpdateCommentRequestHandler(
        IRepositoryWithEvents<Comment> repository,
        ICurrentUser currentUser,
        IStringLocalizer<UpdateGroupClassRequestHandler> localizer)
    {
        (_repository, _t, _currentUser) = (repository, localizer, currentUser);
    }

    public async Task<DefaultIdType> Handle(UpdateCommentRequest request, CancellationToken cancellationToken)
    {
        var comment = await _repository.GetByIdAsync(request.Id, cancellationToken);
        var userId = _currentUser.GetUserId();

        _ = comment ?? throw new NotFoundException(_t["Comment {0} Not Found.", request.Id]);

        comment.Update(request.PostId, request.Content ?? string.Empty, request.ParentId);

        await _repository.UpdateAsync(comment, cancellationToken);

        return comment.Id;
    }
}

