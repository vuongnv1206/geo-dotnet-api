using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Class.UserClasses;
using FSH.WebApi.Application.Class.UserStudents;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;
using Microsoft.AspNetCore.Mvc;
using FSH.WebApi.Application.Class.UserStudents.Dto;

namespace FSH.WebApi.Host.Controllers.Class;
public class UserStudentController : VersionedApiController
{

    [HttpPost("search")]
    [MustHavePermission(FSHAction.Search, FSHResource.Classes)]
    [OpenApiOperation("Search user student using available filters.", "")]
    public Task<PaginationResponse<UserStudentDto>> SearchAsync(SearchUserStudentRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("student-in-class")]
    [MustHavePermission(FSHAction.Create, FSHResource.Classes)]
    [OpenApiOperation("Add a student in class")]
    public Task AddStudentInClass(CreateUserStudentRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.Classes)]
    [OpenApiOperation("Update a userStudent.", "")]
    public async Task<ActionResult<Guid>> UpdateTeacherRegistrationStatusAsync(UpdateStudentWhenUserRegisterRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("remove-user-in-class")]
    [MustHavePermission(FSHAction.Delete, FSHResource.Classes)]
    [OpenApiOperation("Delete user in class.", "")]
    public Task DeleteUserInClass(Guid id)
    {
        return Mediator.Send(new DeleteStudentRequest(id));
    }

    [HttpPut("student-in-class/{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.Classes)]
    [OpenApiOperation("Update information of teacher in team")]
    public async Task<ActionResult<Guid>> UpdateInformationStudentInClass(UpdateInformationStudentRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }
}
