using FSH.WebApi.Application.Catalog.Brands;
using FSH.WebApi.Application.Catalog.Products;
using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Class.GroupClasses;
using Microsoft.AspNetCore.Mvc;

namespace FSH.WebApi.Host.Controllers.Class;
public class ClassController : VersionedApiController
{
    [HttpPost("search")]
    [MustHavePermission(FSHAction.Search, FSHResource.Classes)]
    [OpenApiOperation("Search class using available filters.", "")]
    public Task<PaginationResponse<ClassDto>> SearchAsync(SearchClassesRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [MustHavePermission(FSHAction.View, FSHResource.Classes)]
    [OpenApiOperation("Get class details.", "")]
    public Task<ClassDto> GetAsync(Guid id)
    {
        return Mediator.Send(new GetClassesRequest(id));
    }

    [HttpPost]
    [MustHavePermission(FSHAction.Create, FSHResource.Classes)]
    [OpenApiBodyParameter("Create new classes.", "")]
    public Task<Guid> CreateAsync(CreateClassRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.Classes)]
    [OpenApiBodyParameter("Update a classes.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateClassRequest request, Guid id)
    {
        return id != request.Id ? BadRequest() : Ok(await Mediator.Send(request));
    }
}
