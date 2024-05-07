using FSH.WebApi.Application.Catalog.Brands;
using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Application.TeacherGroup.TeacherInGroups;
using FSH.WebApi.Domain.TeacherGroup;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FSH.WebApi.Host.Controllers.TeacherGroup;

public class GroupTeachersController : VersionedApiController
{
    [HttpPost("search")]
    [MustHavePermission(FSHAction.Search, FSHResource.GroupTeachers)]
    [OpenApiOperation("Search groupTeachers using available filters.", "")]
    public Task<PaginationResponse<GroupTeacherDto>> SearchAsync(SearchGroupTeachersRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [MustHavePermission(FSHAction.View, FSHResource.GroupTeachers)]
    [OpenApiOperation("Get groupTeacher details.", "")]
    public Task<GroupTeacherDto> GetAsync(Guid id)
    {
        return Mediator.Send(new GetGroupTeacherRequest(id));
    }

    [HttpPost]
    [MustHavePermission(FSHAction.Create, FSHResource.GroupTeachers)]
    [OpenApiOperation("Create a new groupTeacher.", "")]
    public Task<Guid> CreateAsync(CreateGroupTeacherRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.GroupTeachers)]
    [OpenApiOperation("Update a groupTeacher.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateGroupTeacherRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(FSHAction.Delete, FSHResource.GroupTeachers)]
    [OpenApiOperation("Delete a groupTeacher.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteGroupTeacherRequest(id));
    }

    [HttpPost("add-teacher-into-group")]
    [MustHavePermission(FSHAction.Update, FSHResource.GroupTeachers)]
    [OpenApiOperation("Add a teacher into group")]
    public Task AddTeacherIntoTeam(AddTeacherIntoGroupRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpDelete("remove-teacher-into-group")]
    [MustHavePermission(FSHAction.Delete, FSHResource.GroupTeachers)]
    [OpenApiOperation("Remove a teacher in group")]
    public Task RemoveTeacherInGroup(RemoveTeacherInGroupRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpDelete("remove-teacher-in-team")]
    [MustHavePermission(FSHAction.Delete, FSHResource.GroupTeachers)]
    [OpenApiOperation("Remove a teacher in team")]
    public Task RemoveTeacherInTeam(RemoveTeacherInTeamRequest request)
    {
        return Mediator.Send(request);
    }
}
