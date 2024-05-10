using FSH.WebApi.Application.Catalog.Brands;
using FSH.WebApi.Application.Examination.PaperLabels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FSH.WebApi.Host.Controllers.Examination;
public class PaperLabelsController : VersionedApiController
{
    [HttpPost("Search")]
    [OpenApiOperation("")]
    public Task<PaginationResponse<PaperLabelDto>> SearchPaperLabel(SearchPaperLabelRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [OpenApiOperation("Get paper label details.", "")]
    public Task<PaperLabelDto> GetAsync(Guid id)
    {
        return Mediator.Send(new GetPaperLabelRequest(id));
    }


    [HttpPost]
    [OpenApiOperation("Create paper label")]
    public Task<Guid> CreateAsync(CreatePaperLabelRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [OpenApiOperation("")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdatePaperLabelRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [OpenApiOperation("")]
    public Task RemoveAsync(Guid id)
    {
        return Mediator.Send(new RemovePaperLabelRequest(id));
    }
}
