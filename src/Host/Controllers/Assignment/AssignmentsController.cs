using FSH.WebApi.Application.Assignments;
using FSH.WebApi.Application.Assignments.Dtos;

namespace FSH.WebApi.Host.Controllers.Assignment;

public class AssignmentsController : VersionedApiController
{
    [HttpPost("search")]
    [MustHavePermission(FSHAction.Search, FSHResource.Assignments)]
    [OpenApiOperation("Search assignments using available filters.", "")]
    public Task<PaginationResponse<AssignmentDto>> SearchAsync(SearchAssignmentsRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [MustHavePermission(FSHAction.View, FSHResource.Assignments)]
    [OpenApiOperation("Get assignment details.", "")]
    public Task<AssignmentDetailsDto> GetAsync(Guid id)
    {
        return Mediator.Send(new GetAssignmentRequest(id));
    }

    [HttpGet("dapper")]
    [MustHavePermission(FSHAction.View, FSHResource.Assignments)]
    [OpenApiOperation("Get assignment details via dapper.", "")]
    public Task<AssignmentDto> GetDapperAsync(Guid id)
    {
        return Mediator.Send(new GetAssignmentViaDapperRequest(id));
    }

    [HttpPost]
    [MustHavePermission(FSHAction.Create, FSHResource.Assignments)]
    [OpenApiOperation("Create a new assignment.", "")]
    public Task<Guid> CreateAsync(CreateAssignmentRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.Assignments)]
    [OpenApiOperation("Update a assignment.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateAssignmentRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(FSHAction.Delete, FSHResource.Assignments)]
    [OpenApiOperation("Delete a assignment.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteAssignmentRequest(id));
    }

}
