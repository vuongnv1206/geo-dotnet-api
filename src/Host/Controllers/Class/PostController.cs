using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Class.Comments;
using FSH.WebApi.Application.Class.GroupClasses;
using FSH.WebApi.Application.Class.New;
using FSH.WebApi.Application.Class.New.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FSH.WebApi.Host.Controllers.Class;
public class PostController : VersionedApiController
{

    [HttpPost("search")]
    [MustHavePermission(FSHAction.Search, FSHResource.Classes)]
    [OpenApiOperation("Get all news in the class.", "")]
    public Task<PaginationResponse<PostDto>> SearchAsync(GetPostRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost]
    [MustHavePermission(FSHAction.Create, FSHResource.News)]
    [OpenApiOperation("Create a News.", "")]
    public Task<Guid> CreateAsync(CreatePostRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.News)]
    [OpenApiOperation("Update a News.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdatePostRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(FSHAction.Delete, FSHResource.News)]
    [OpenApiOperation("Delete a News.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteNewsRequest(id));
    }


    [HttpPost("comments")]
    [MustHavePermission(FSHAction.Create, FSHResource.News)]
    [OpenApiOperation("Create a Comment in the Post.", "")]
    public Task<Guid> CreateCommentAsync(CreateCommentRequest request)
    {
        return Mediator.Send(request);
    }
}
