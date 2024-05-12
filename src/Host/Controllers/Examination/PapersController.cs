using FSH.WebApi.Application.Examination.Papers;

namespace FSH.WebApi.Host.Controllers.Examination;
public class PapersController : VersionedApiController
{
    [HttpPost]
    [OpenApiOperation("Create a paper.")]
    public Task<PaperDto> CreateAsync(CreatePaperRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [OpenApiOperation("Update information of paper")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdatePaperRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }
}
