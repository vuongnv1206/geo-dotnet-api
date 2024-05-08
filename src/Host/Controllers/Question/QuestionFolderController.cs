using FSH.WebApi.Application.Catalog.Products;
using FSH.WebApi.Application.Dashboard;
using FSH.WebApi.Application.Questions;
using FSH.WebApi.Application.Questions.Dtos;

namespace FSH.WebApi.Host.Controllers.Question;

public class QuestionFolderController : VersionedApiController
{
    [HttpGet]
    [MustHavePermission(FSHAction.View, FSHResource.QuestionFolders)]
    [OpenApiOperation("Get tree of question folders.", "")]
    public Task<QuestionTreeDto> GetAsync(Guid? parentId)
    {
        return Mediator.Send(new GetFolderTreeRequest(parentId));
    }

    [HttpPost]
    [MustHavePermission(FSHAction.Create, FSHResource.QuestionFolders)]
    [OpenApiOperation("Create a new question folder.", "")]
    public Task<Guid> CreateAsync(CreateFolderRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.QuestionFolders)]
    [OpenApiOperation("Update a question folder.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateFolderRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(FSHAction.Delete, FSHResource.QuestionFolders)]
    [OpenApiOperation("Delete a question folder.", "")]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteFolderRequest(id));
    }

    [HttpPost("share/{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.QuestionFolders)]
    [OpenApiOperation("Share a question folder.", "")]
    public async Task<ActionResult<Guid>> ShareAsync(ShareQuestionFolderRequest request, Guid id)
    {
        return id != request.FolderId
            ? BadRequest()
            : await Mediator.Send(request);
    }

}