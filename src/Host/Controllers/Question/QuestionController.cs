using FSH.WebApi.Application.Questions;
using FSH.WebApi.Application.Questions.Dtos;

namespace FSH.WebApi.Host.Controllers.Question;

public class QuestionController : VersionedApiController
{
    [HttpPost("search")]
    [MustHavePermission(FSHAction.View, FSHResource.Question)]
    [OpenApiOperation("search questions using available filters.", "")]
    public Task<PaginationResponse<QuestionDto>> SearchAsync(SearchQuestionsRequest request)
    {
        return Mediator.Send(request);
    }
}