using FSH.WebApi.Application.Questions;
using FSH.WebApi.Application.Questions.Dtos;
using MediatR;

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
    [OpenApiOperation("Create question list.", "")]
    public async Task<List<Guid>> CreateAsync(CreateQuestionRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpDelete("id:guid")]
    [MustHavePermission(FSHAction.Delete, FSHResource.Question)]
    [OpenApiOperation("Delete a questions.", "")]
    public async Task<Guid> DeleteAsync(Guid id)
    {
        return await Mediator.Send(new DeleteQuestionRequest(id));
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Create, FSHResource.Question)]
    [OpenApiOperation("Update a question.", "")]
    public async Task<ActionResult<Guid>> UpdateAsync(Guid id, UpdateQuestionRequest request)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }
    
}