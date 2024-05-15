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

    [HttpPost]
    [MustHavePermission(FSHAction.Create, FSHResource.Question)]
    [OpenApiOperation("Create questionn list.", "")]
    public async Task<List<Guid>> CreateAsync(CreateQuestionRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Create, FSHResource.Question)]
    [OpenApiOperation("Update a question.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(Guid id, UpdateAQuestionRequest request)
    {
        return id != request.Question.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }
}