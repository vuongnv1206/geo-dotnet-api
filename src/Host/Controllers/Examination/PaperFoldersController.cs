using FSH.WebApi.Application.Examination.PaperFolders;

namespace FSH.WebApi.Host.Controllers.Examination;
public class PaperFoldersController : VersionedApiController
{
    [HttpPost]
    [OpenApiOperation("Create a new paperFolder.", "")]
    public Task<Guid> CreateAsync(CreatePaperFolderRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("search")]
    [MustHavePermission(FSHAction.View, FSHResource.GroupTeachers)]
    [OpenApiOperation("Search paper folder using available folder", "")]
    public Task<PaginationResponse<PaperFolderDto>> SearchAsync(SearchPaperFolderRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpDelete("{id:guid}")]
    [OpenApiOperation("Delete a paperFolder.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeletePaperFolderRequest(id));
    }

    [HttpPut("{id:guid}")]
    [OpenApiOperation("Update information of paper folder.")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdatePaperFolderRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpPost("{id:guid}/share-paper-folder")]
    [OpenApiOperation("Share paper folder.")]
    public async Task<ActionResult<Guid>> ShareFolder(Guid id, SharePaperFolderRequest request)
    {
        return id != request.FolderId
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpPost("shared")]
    [OpenApiOperation("Search shared paper folder using available folder", "")]
    public Task<PaginationResponse<PaperFolderDto>> SearchShared(SearchSharedPaperFolderRequest request)
    {
        return Mediator.Send(request);
    }

}
