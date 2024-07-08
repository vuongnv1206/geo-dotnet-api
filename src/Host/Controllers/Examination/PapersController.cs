using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Examination.Papers;

namespace FSH.WebApi.Host.Controllers.Examination;
public class PapersController : VersionedApiController
{
    [HttpPost("Search")]
    [OpenApiOperation("Search paper using available filter", "")]
    [MustHavePermission(FSHAction.View, FSHResource.Papers)]
    public Task<PaginationResponse<PaperInListDto>> SearchPaper(SearchPaperRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost]
    [OpenApiOperation("Create a paper.")]
    [MustHavePermission(FSHAction.Create, FSHResource.Papers)]
    public Task<Guid> CreateAsync(CreatePaperRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [OpenApiOperation("Get paper details.", "")]
    [MustHavePermission(FSHAction.View, FSHResource.Papers)]
    public Task<PaperDto> GetAsync(Guid id)
    {
        return Mediator.Send(new GetPaperByIdRequest(id));
    }

    [HttpPut("{id:guid}")]
    [OpenApiOperation("Update information of paper")]
    [MustHavePermission(FSHAction.Update, FSHResource.Papers)]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdatePaperRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [OpenApiOperation("Delete a paper")]
    [MustHavePermission(FSHAction.Delete, FSHResource.Papers)]
    public async Task<Guid> DeleteAsync(Guid id)
    {
        return await Mediator.Send(new DeletePaperRequest(id));
    }

    [HttpPut("{id:guid}/questions")]
    [OpenApiOperation("Update questions in a paper")]
    [MustHavePermission(FSHAction.Update, FSHResource.Papers)]
    public async Task<IActionResult> UpdateQuestionsInPaperAsync(Guid id, [FromBody] UpdateQuestionsInPaperRequest request)
    {
        if (id != request.PaperId)
        {
            return BadRequest("Paper ID in the request does not match the ID in the route.");
        }

        return Ok(await Mediator.Send(request));
    }

    [HttpPost("Shared")]
    [MustHavePermission(FSHAction.View, FSHResource.Papers)]
    [OpenApiOperation("")]
    public Task<List<PaperInListDto>> SearchSharedPaper(SearchSharedPaperRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("{id:guid}/share-paper")]
    [OpenApiOperation("Share paper.")]
    [MustHavePermission(FSHAction.Update, FSHResource.Papers)]
    public async Task<ActionResult<Guid>> ShareFolder(Guid id, SharePaperRequest request)
    {
        return id != request.PaperId
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

}
