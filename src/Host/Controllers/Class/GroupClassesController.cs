using FSH.WebApi.Application.Catalog.Brands;
using FSH.WebApi.Application.Class.GroupClasses;
using Microsoft.AspNetCore.Mvc;

namespace FSH.WebApi.Host.Controllers.Class;
public class GroupClassesController : VersionedApiController
{

    [HttpPost]
    [MustHavePermission(FSHAction.Create, FSHResource.GroupClasses)]
    [OpenApiOperation("Create a new GroupClasses.", "")]
    public Task<Guid> CreateAsync(CreateGroupClassesRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.GroupClasses)]
    [OpenApiOperation("Update a GroupClass.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateGroupClassRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(FSHAction.Delete, FSHResource.GroupClasses)]
    [OpenApiOperation("Delete a GroupClass.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteGroupClassRequest(id));
    }

}
