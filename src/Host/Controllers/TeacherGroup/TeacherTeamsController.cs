using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;

namespace FSH.WebApi.Host.Controllers.TeacherGroup;

public class TeacherTeamsController : VersionedApiController
{
    [HttpPost("search")]
    [MustHavePermission(FSHAction.View, FSHResource.GroupTeachers)]
    [OpenApiOperation("Search teacherTeam using available filters.", "")]
    public Task<PaginationResponse<TeacherTeamDto>> SearchAsync(SearchTeacherTeamsRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [MustHavePermission(FSHAction.View, FSHResource.GroupTeachers)]
    [OpenApiOperation("Get teacherTeam details.", "")]
    public Task<TeacherTeamDto> GetAsync(Guid id)
    {
        return Mediator.Send(new GetTeacherTeamRequest(id));
    }


    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.GroupTeachers)]
    [OpenApiOperation("Update a teacherTeam.", "")]
    public async Task<ActionResult<Guid>> UpdateTeacherRegistrationStatusAsync(UpdateTeacherRegistrationStatusRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }


    [HttpPost("teacher-in-team")]
    [MustHavePermission(FSHAction.Update, FSHResource.GroupTeachers)]
    [OpenApiOperation("Add a teacher in my teacher team")]
    public Task AddTeacherInTeacherTeam(AddTeacherIntoTeacherTeamRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(FSHAction.Delete, FSHResource.GroupTeachers)]
    [OpenApiOperation("Remove a teacher in team")]
    public Task RemoveTeacherInTeam(Guid id)
    {
        return Mediator.Send(new RemoveTeacherInTeamRequest(id));
    }

    [HttpPut("teacher-in-team/{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.GroupTeachers)]
    [OpenApiOperation("Update information of teacher in team")]
    public async Task<ActionResult<Guid>> UpdateInformationTeacherInTeam(UpdateInformationTeacherInTeamRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }
}
