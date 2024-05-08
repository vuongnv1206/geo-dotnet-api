﻿using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
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
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateTeacherRegistrationStatusRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }


    [HttpPost("add-teacher-into-team")]
    [MustHavePermission(FSHAction.Update, FSHResource.GroupTeachers)]
    [OpenApiOperation("Add a teacher in my teacher team")]
    public Task AddTeacherInTeacherTeam(AddTeacherIntoTeacherTeamRequest request)
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
