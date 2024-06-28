using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.GroupClasses;
using FSH.WebApi.Application.Class.UserClasses;
using FSH.WebApi.Application.Assignments.AssignmentClasses;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Application.Class.UserClasses.Dto;
using FSH.WebApi.Application.Class.SharedClasses;
using FSH.WebApi.Application.Class.GroupClasses.Dto;

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


    [HttpDelete("{id:guid}")]
    [MustHavePermission(FSHAction.Delete, FSHResource.Classes)]
    [OpenApiOperation("Delete a Class.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteClassRequest(id));
    }


    [HttpDelete("remove-user-in-class")]
    [MustHavePermission(FSHAction.Delete, FSHResource.UserClasses)]
    [OpenApiOperation("Delete user in class.", "")]
    public Task DeleteUserInClass(DeleteUserInClassRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("remove-assignment-from-class")]
    [MustHavePermission(FSHAction.Delete, FSHResource.Classes)]
    [OpenApiOperation("Remove assignment from class")]
    public Task RemoveAssignmentFromClass(RemoveAssignmentFromClassRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("shared-classes")]
    [MustHavePermission(FSHAction.View, FSHResource.Classes)]
    [OpenApiOperation("Get all shared classes")]
    public Task<PaginationResponse<GroupClassDto>> GetSharedClasses(SearchSharedClassRequest request)
    {
        return Mediator.Send(request);
    }
}
