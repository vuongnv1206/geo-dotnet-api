using FSH.WebApi.Application.Assignments;
using FSH.WebApi.Application.Assignments.AssignmentStudent;
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

    [HttpPost("mark")]
    [MustHavePermission(FSHAction.Update, FSHResource.Assignments)]
    [OpenApiOperation("Mark assignment.", "")]
    public Task<Guid> MarkAsync(MarkAssignmentRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("submit")]
    [MustHavePermission(FSHAction.Update, FSHResource.SubmitAssignment)]
    [OpenApiOperation("Submit assignment.", "")]
    public Task SubmitAsync(SubmitAssignmentRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("submission/{id:guid}")]
    [MustHavePermission(FSHAction.View, FSHResource.Assignments)]
    [OpenApiOperation("Get submission assignment details.", "")]
    public Task<List<SubmissionAssignmentDto>> GetSubmissionAsync(Guid id)
    {
        return Mediator.Send(new GetSubmissionAssignmentRequest(id));
    }

}

