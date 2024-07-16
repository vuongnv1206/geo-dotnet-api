using FSH.WebApi.Application.Examination.PaperLabels;

namespace FSH.WebApi.Host.Controllers.Examination;
public class PaperLabelsController : VersionedApiController
{
    [HttpPost("Search")]
    [OpenApiOperation("Search paper label using available filter", "")]
    [MustHavePermission(FSHAction.View, FSHResource.PaperLabels)]
    public Task<PaginationResponse<PaperLabelDto>> SearchPaperLabel(SearchPaperLabelRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [OpenApiOperation("Get paper label details.", "")]
    [MustHavePermission(FSHAction.View, FSHResource.PaperLabels)]
    public Task<PaperLabelDto> GetAsync(Guid id)
    {
        return Mediator.Send(new GetPaperLabelRequest(id));
    }

    [HttpPost]
    [OpenApiOperation("Create paper label")]
    [MustHavePermission(FSHAction.Create, FSHResource.PaperLabels)]
    public Task<Guid> CreateAsync(CreatePaperLabelRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.PaperLabels)]
    [OpenApiOperation("Update a label")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdatePaperLabelRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [OpenApiOperation("Remove a label")]
    [MustHavePermission(FSHAction.Delete, FSHResource.PaperLabels)]
    public Task RemoveAsync(Guid id)
    {
        return Mediator.Send(new RemovePaperLabelRequest(id));
    }
}
