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

}
