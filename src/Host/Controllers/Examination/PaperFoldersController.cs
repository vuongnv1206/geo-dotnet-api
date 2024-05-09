using FSH.WebApi.Application.Examination;

namespace FSH.WebApi.Host.Controllers.Examination;
public class PaperFoldersController : VersionedApiController
{
    [HttpPost]
    [OpenApiOperation("Create a new paperFolder.", "")]
    public Task<Guid> CreateAsync(CreatePaperFolderRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpDelete("{id:guid}")]
    [OpenApiOperation("Delete a paperFolder.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeletePaperFolderRequest(id));
    }
}
