using FSH.WebApi.Application.Questions.QuestionLabel;


namespace FSH.WebApi.Host.Controllers.Question;

public class QuestionLabelController : VersionedApiController
{
    [HttpPost("Search")]
    [OpenApiOperation("Search question label using available filter", "")]
    [MustHavePermission(FSHAction.View, FSHResource.QuestionLabel)]
    public Task<PaginationResponse<QuestionLabelDto>> SearchQuestionLabel(SearchQuestionLabelRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [OpenApiOperation("Get question label details.", "")]
    [MustHavePermission(FSHAction.View, FSHResource.QuestionLabel)]
    public Task<QuestionLabelDto> GetAsync(Guid id)
    {
        return Mediator.Send(new GetQuestionLabelRequest(id));
    }

    [HttpPost]
    [OpenApiOperation("Create question label")]
    [MustHavePermission(FSHAction.Create, FSHResource.QuestionLabel)]
    public Task<Guid> CreateAsync(CreateQuestionLabelRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.QuestionLabel)]
    [OpenApiOperation("Update a label")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateQuestionLabelRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [OpenApiOperation("Remove a label")]
    [MustHavePermission(FSHAction.Delete, FSHResource.QuestionLabel)]
    public Task RemoveAsync(Guid id)
    {
        return Mediator.Send(new RemoveQuestionLabelRequest(id));
    }
}
