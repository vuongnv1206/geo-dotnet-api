using FSH.WebApi.Application.Examination.PaperFolders;

namespace FSH.WebApi.Host.Controllers.Examination;
public class PaperFoldersController : VersionedApiController
{
    [HttpPost]
    [MustHavePermission(FSHAction.View, FSHResource.PaperFolders)]
    [OpenApiOperation("Create a new paperFolder.", "")]
    public Task<Guid> CreateAsync(CreatePaperFolderRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("search")]
    [MustHavePermission(FSHAction.View, FSHResource.PaperFolders)]
    [OpenApiOperation("Search paper folder using available filter", "")]
    public Task<PaginationResponse<PaperFolderDto>> SearchAsync(SearchPaperFolderRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(FSHAction.Delete, FSHResource.PaperFolders)]
    [OpenApiOperation("Delete a paperFolder.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeletePaperFolderRequest(id));
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.PaperFolders)]
    [OpenApiOperation("Update information of paper folder.")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdatePaperFolderRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpPost("{id:guid}/share-paper-folder")]
    [OpenApiOperation("Share paper folder.")]
    [MustHavePermission(FSHAction.Update, FSHResource.PaperFolders)
    public async Task<ActionResult<Guid>> ShareFolder(Guid id, SharePaperFolderRequest request)
    {
        return id != request.FolderId
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpPost("shared")]
    [OpenApiOperation("Search shared paper folder using available folder", "")]
    [MustHavePermission(FSHAction.View, FSHResource.PaperFolders)]
    public Task<List<PaperFolderDto>> SearchShared(SearchSharedPaperFolderRequest request)
    {
        return Mediator.Send(request);
    }
    [HttpGet("{id}/parents")]
    [OpenApiOperation("Get list parents.")]
    [MustHavePermission(FSHAction.View, FSHResource.PaperFolders)]
    public async Task<ActionResult<List<PaperFolderDto>>> GetPaperFolderParents(Guid id)
    {
        var result = await Mediator.Send(new GetPaperFolderParentsRequest(id));
        return Ok(result);
    }

}
