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

    [HttpPost("search-my-requests")]
    [OpenApiOperation("search questions created by the current user.", "")]
    public Task<PaginationResponse<QuestionDto>> SearchMyRequestsAsync(SearchMyRequestQuestionsRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("search-approval")]
    [OpenApiOperation("search questions that need approval.", "")]
    public Task<PaginationResponse<QuestionDto>> SearchApprovalAsync(SearchApprovalQuestionsRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("search-my-deleted")]
    [OpenApiOperation("search questions deleted by the current user.", "")]
    public Task<PaginationResponse<QuestionDto>> SearchMyDeletedAsync(SearchMyDeletedQuestions request)
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

    [HttpDelete("{id:guid}")]
    [MustHavePermission(FSHAction.Delete, FSHResource.Question)]
    [OpenApiOperation("Delete a questions.", "")]
    public async Task<Guid> DeleteAsync(Guid id)
    {
        return await Mediator.Send(new DeleteQuestionRequest(id));
    }

    [HttpDelete]
    [MustHavePermission(FSHAction.Delete, FSHResource.Question)]
    [OpenApiOperation("Delete a list of questions.", "")]
    public async Task<List<Guid>> DeleteListAsync(DeleteQuestionsRequest request)
    {
        return await Mediator.Send(request);
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

    [HttpPut("restore-deleted")]
    [OpenApiOperation("Restore deleted questions.", "")]
    public async Task<List<Guid>> RestoreDeletedAsync(RestoreDeletedQuestionsRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPost("read-from-file")]
    [MustHavePermission(FSHAction.Create, FSHResource.Question)]
    [OpenApiOperation("read questions from .docx, .txt file.", "")]
    public async Task<string[]> ImportAsync([FromForm] ReadQuestionsFromFileRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPut("approve")]
    [OpenApiOperation("Approve questions.", "")]
    public async Task<List<Guid>> ApproveAsync(ApproveQuestionsRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPut("reject")]
    [OpenApiOperation("Reject questions.", "")]
    public async Task<List<Guid>> RejectAsync(RejectQuestionsRequest request)
    {
        return await Mediator.Send(request);
    }
    [HttpPost("Clone")]
    [MustHavePermission(FSHAction.Create, FSHResource.Question)]
    [OpenApiOperation("Create question clone.", "")]
    public async Task<Guid> CreateAsync(CreateQuestionCloneRequest request)
    {
        return await Mediator.Send(request);
    }


}