using FSH.WebApi.Application.Class.New;

namespace FSH.WebApi.Host.Controllers.Class;
public class NewsReactionController : VersionedApiController
{
    [HttpPost]
    [MustHavePermission(FSHAction.Create, FSHResource.NewsReaction)]
    [OpenApiOperation("Create a NewsReaction.", "")]
    public Task<Guid> CreateAsync(CreateNewsReactionsRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpDelete]
    [MustHavePermission(FSHAction.Delete, FSHResource.NewsReaction)]
    [OpenApiOperation("Remove a news likes", "")]
    public Task RemoveLikeInTheNews(DeleteNewsReactionRequest request)
    {
        return Mediator.Send(request);
    }

}
