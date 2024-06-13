using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Class.GroupClasses;
using FSH.WebApi.Application.Class.GroupClasses.Dto;

namespace FSH.WebApi.Host.Controllers.Class;
public class GroupClassesController : VersionedApiController
{

    [HttpPost("search")]
    [MustHavePermission(FSHAction.Search, FSHResource.GroupClasses)]
    [OpenApiOperation("Search groupClass using available filters.", "")]
    public Task<List<GroupClassDto>> SearchAsync(SearchGroupClassRequest request)
    {
        return Mediator.Send(request);
    }
    [HttpGet]
    [MustHavePermission(FSHAction.View, FSHResource.GroupClasses)]
    [OpenApiOperation("Get list GroupClasses of user.", "")]
    public Task<List<GroupClassDto>> GetAsync()
    {
        return Mediator.Send(new GroupClassRequest());
    }

    [HttpGet("group-class-detail")]
    [MustHavePermission(FSHAction.View, FSHResource.GroupClasses)]
    [OpenApiOperation("Get list Class by groupClass.", "")]
    public Task<List<GroupClassOfClassDto>> GetGroupClassDetailAsync()
    {
        return Mediator.Send(new GetGroupClassRequest());
    }

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
