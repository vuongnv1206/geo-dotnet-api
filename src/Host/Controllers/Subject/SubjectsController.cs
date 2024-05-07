using FSH.WebApi.Application.Subjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FSH.WebApi.Host.Controllers.Subject;

public class SubjectsController : VersionedApiController
{
    [HttpPost("search")]
    [MustHavePermission(FSHAction.Search, FSHResource.Subjects)]
    [OpenApiOperation("Search subjects using available filters.", "")]
    public Task<PaginationResponse<SubjectDto>> SearchAsync(SearchSubjectsRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [MustHavePermission(FSHAction.View, FSHResource.Subjects)]
    [OpenApiOperation("Get subject details.", "")]
    public Task<SubjectDto> GetAsync(Guid id)
    {
        return Mediator.Send(new GetSubjectRequest(id));
    }

    [HttpPost]
    [MustHavePermission(FSHAction.Create, FSHResource.Subjects)]
    [OpenApiOperation("Create a new subject.", "")]
    public Task<Guid> CreateAsync(CreateSubjectRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.Subjects)]
    [OpenApiOperation("Update a subject.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateSubjectRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(FSHAction.Delete, FSHResource.Subjects)]
    [OpenApiOperation("Delete a subject.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteSubjectRequest(id));
    }

    [HttpPost("generate-random")]
    [MustHavePermission(FSHAction.Generate, FSHResource.Subjects)]
    [OpenApiOperation("Generate a number of random subjects.", "")]
    public Task<string> GenerateRandomAsync(GenerateRandomSubjectRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpDelete("delete-random")]
    [MustHavePermission(FSHAction.Clean, FSHResource.Subjects)]
    [OpenApiOperation("Delete the subjects generated with the generate-random call.", "")]
    [ApiConventionMethod(typeof(FSHApiConventions), nameof(FSHApiConventions.Search))]
    public Task<string> DeleteRandomAsync()
    {
        return Mediator.Send(new DeleteRandomSubjectRequest());
    }
}