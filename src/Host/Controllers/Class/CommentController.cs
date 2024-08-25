using FSH.WebApi.Application.Class.Comments;

namespace FSH.WebApi.Host.Controllers.Class;
public class CommentController : VersionedApiController
{
    [HttpPost]
    [MustHavePermission(FSHAction.Create, FSHResource.CommentPost)]
    [OpenApiOperation("Create a comment.", "")]
    public Task<Guid> CreateCommentAsync(CreateCommentRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.CommentPost)]
    [OpenApiOperation("Update a comment.", "")]
    public async Task<ActionResult<Guid>> UpdateCommentAsync(UpdateCommentRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(FSHAction.Delete, FSHResource.CommentPost)]
    [OpenApiOperation("Delete a comment.", "")]
    public Task<Guid> DeleteCommentAsync(Guid id)
    {
        return Mediator.Send(new DeleteCommentRequest(id));
    }

    [HttpPost("like")]
    [MustHavePermission(FSHAction.Create, FSHResource.NewsReaction)]
    [OpenApiOperation("Like a comment.", "")]
    public Task<Guid> CreateCommentLikeAsync(CreateCommentLikeRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("dislike")]
    [MustHavePermission(FSHAction.Delete, FSHResource.NewsReaction)]
    [OpenApiOperation("DisLike a comment", "")]
    public Task RemoveLikeInTheComment(DeleteCommentLikeRequest request)
    {
        return Mediator.Send(request);
    }
}
