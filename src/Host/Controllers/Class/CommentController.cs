using FSH.WebApi.Application.Class.Comments;
using FSH.WebApi.Application.Class.New.Dto;
using FSH.WebApi.Application.Class.New;
using Microsoft.AspNetCore.Mvc;

namespace FSH.WebApi.Host.Controllers.Class;
public class CommentController : VersionedApiController
{
    [HttpPost]
    [MustHavePermission(FSHAction.Create, FSHResource.News)]
    [OpenApiOperation("Create a Comment.", "")]
    public Task<Guid> CreateCommentAsync(CreateCommentRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.News)]
    [OpenApiOperation("Update a Comment.", "")]
    public async Task<ActionResult<Guid>> UpdateCommentAsync(UpdateCommentRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(FSHAction.Delete, FSHResource.News)]
    [OpenApiOperation("Delete a comment.", "")]
    public Task<Guid> DeleteCommentAsync(Guid id)
    {
        return Mediator.Send(new DeleteCommentRequest(id));
    }
}
