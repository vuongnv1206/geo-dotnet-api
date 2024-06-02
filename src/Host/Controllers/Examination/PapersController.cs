using FSH.WebApi.Application.Examination.Papers;

namespace FSH.WebApi.Host.Controllers.Examination;
public class PapersController : VersionedApiController
{
    [HttpPost("Search")]
    [OpenApiOperation("")]
    public Task<List<PaperInListDto>> SearchPaperLabel(SearchPaperRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost]
    [OpenApiOperation("Create a paper.")]
    public Task<PaperDto> CreateAsync(CreatePaperRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [OpenApiOperation("Get paper details.", "")]
    public Task<PaperDto> GetAsync(Guid id)
    {
        return Mediator.Send(new GetPaperByIdRequest(id));
    }

    [HttpPut("{id:guid}")]
    [OpenApiOperation("Update information of paper")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdatePaperRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [OpenApiOperation("Delete a paper")]
    public async Task<Guid> DeleteAsync(Guid id)
    {
        return await Mediator.Send(new DeletePaperRequest(id));
    }
}
