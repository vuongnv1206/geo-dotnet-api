using FSH.WebApi.Application.Catalog.Brands;
using FSH.WebApi.Application.Catalog.Products;
using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.GroupClasses;
using FSH.WebApi.Application.Class.New;
using FSH.WebApi.Application.Class.UserClasses;
using FSH.WebApi.Domain.Class;
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

    [HttpGet]
    [MustHavePermission(FSHAction.View, FSHResource.Classes)]
    [OpenApiOperation("Get class of user", "")]
    public Task<List<ClassDto>> GetClassByUserAsync()
    {
        return Mediator.Send(new GetClassOfUserRequest());
    }

    [HttpGet("{id:guid}")]
    [MustHavePermission(FSHAction.View, FSHResource.Classes)]
    [OpenApiOperation("Get class details.", "")]
    public Task<ClassDto> GetAsync(Guid id)
    {
        return Mediator.Send(new GetClassesRequest(id));
    }


    [HttpGet("get-class-by-group-class")]
    [MustHavePermission(FSHAction.View, FSHResource.Classes)]
    [OpenApiOperation("Get class by group class.", "")]
    public Task<List<Classes>> GetClassByGroupClassAsync(Guid groupClassId)
    {
        return Mediator.Send(new GetClassByGroupClassRequest(groupClassId));
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

    [HttpGet("getall-user-in-class")]
    [MustHavePermission(FSHAction.View, FSHResource.UserClasses)]
    [OpenApiOperation("get all user in class", "")]
    public Task<List<UserClass>> GetListAsync(Guid classId)
    {
        return Mediator.Send(new GetUserInClassRequest(classId));
    }

    [HttpPost("add-user-in-class")]
    [MustHavePermission(FSHAction.Create, FSHResource.UserClasses)]
    [OpenApiOperation("Add new user in class.", "")]
    public Task<Guid> CreateAsync(AddUserInClassRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("update-user-in-class")]
    [MustHavePermission(FSHAction.Update, FSHResource.UserClasses)]
    [OpenApiBodyParameter("Update user in class.", "")]
    public Task UpdateUserInClass(UpdateUserInClassRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpDelete("remove-user-in-class")]
    [MustHavePermission(FSHAction.Delete, FSHResource.UserClasses)]
    [OpenApiOperation("Delete user in class.", "")]
    public Task DeleteUserInClass(DeleteUserInClassRequest request)
    {
        return Mediator.Send(request);
    }
}
