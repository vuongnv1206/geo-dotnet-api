using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Class.GroupClasses;
using FSH.WebApi.Application.Class.New;
using Microsoft.AspNetCore.Mvc;

namespace FSH.WebApi.Host.Controllers.Class;
public class NewsController : VersionedApiController
{

    [HttpPost("search")]
    [MustHavePermission(FSHAction.Search, FSHResource.Classes)]
    [OpenApiOperation("Get all news in the class.", "")]
    public Task<PaginationResponse<NewsDto>> SearchAsync(GetNewsRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost]
    [MustHavePermission(FSHAction.Create, FSHResource.News)]
    [OpenApiOperation("Create a News.", "")]
    public Task<Guid> CreateAsync(CreateNewsRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.News)]
    [OpenApiOperation("Update a News.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateNewsRequest request, Guid id)
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
}
