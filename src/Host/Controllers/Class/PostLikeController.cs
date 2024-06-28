using FSH.WebApi.Application.Class.Comments;
using FSH.WebApi.Application.Class.New;

namespace FSH.WebApi.Host.Controllers.Class;
public class PostLikeController : VersionedApiController
{
    [HttpPost]
    [MustHavePermission(FSHAction.Create, FSHResource.NewsReaction)]
    [OpenApiOperation("Create a NewsReaction.", "")]
    public Task<Guid> CreateAsync(CreatePostLikeRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpDelete]
    [MustHavePermission(FSHAction.Delete, FSHResource.NewsReaction)]
    [OpenApiOperation("Remove a news likes", "")]
    public Task RemoveLikeInTheNews(DeletePostReactionRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("like-in-comment")]
    [MustHavePermission(FSHAction.Create, FSHResource.NewsReaction)]
    [OpenApiOperation("Create a NewsReaction.", "")]
    public Task<Guid> CreateCommentLikeAsync(CreateCommentLikeRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpDelete("like-in-comment")]
    [MustHavePermission(FSHAction.Delete, FSHResource.NewsReaction)]
    [OpenApiOperation("Remove a comment likes", "")]
    public Task RemoveLikeInTheComment(DeleteCommentLikeRequest request)
    {
        return Mediator.Send(request);
    }
}
