using FSH.WebApi.Application.Dashboard;
using FSH.WebApi.Application.Questions;

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
}