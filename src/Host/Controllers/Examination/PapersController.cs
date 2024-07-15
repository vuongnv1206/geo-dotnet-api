
using FSH.WebApi.Application.Examination.Papers;
using Microsoft.AspNetCore.Mvc;

namespace FSH.WebApi.Host.Controllers.Examination;
public class PapersController : VersionedApiController
{
    [HttpPost("Search")]
    [OpenApiOperation("Search paper using available filter", "")]
    [MustHavePermission(FSHAction.View, FSHResource.Papers)]
    public Task<PaginationResponse<PaperInListDto>> SearchPaper(SearchPaperRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost]
    [OpenApiOperation("Create a paper.")]
    [MustHavePermission(FSHAction.Create, FSHResource.Papers)]
    public Task<Guid> CreateAsync(CreatePaperRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [OpenApiOperation("Get paper details.", "")]
    [MustHavePermission(FSHAction.View, FSHResource.Papers)]
    public Task<PaperDto> GetAsync(Guid id)
    {
        return Mediator.Send(new GetPaperByIdRequest(id));
    }

    [HttpPut("{id:guid}")]
    [OpenApiOperation("Update information of paper")]
    [MustHavePermission(FSHAction.Update, FSHResource.Papers)]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdatePaperRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [OpenApiOperation("Delete a paper")]
    [MustHavePermission(FSHAction.Delete, FSHResource.Papers)]
    public async Task<Guid> DeleteAsync(Guid id)
    {
        return await Mediator.Send(new DeletePaperRequest(id));
    }

    [HttpPut("{id:guid}/questions")]
    [OpenApiOperation("Update questions in a paper")]
    [MustHavePermission(FSHAction.Update, FSHResource.Papers)]
    public async Task<IActionResult> UpdateQuestionsInPaperAsync(Guid id, [FromBody] AddQuestionsInPaperRequest request)
    {
        if (id != request.PaperId)
        {
            return BadRequest("Paper ID in the request does not match the ID in the route.");
        }

        return Ok(await Mediator.Send(request));
    }

    [HttpPost("Shared")]
    [MustHavePermission(FSHAction.View, FSHResource.Papers)]
    [OpenApiOperation("")]
    public Task<List<PaperInListDto>> SearchSharedPaper(SearchSharedPaperRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("{id:guid}/share-paper")]
    [OpenApiOperation("Share paper.")]
    [MustHavePermission(FSHAction.Update, FSHResource.Papers)]
    public async Task<ActionResult<Guid>> ShareFolder(Guid id, SharePaperRequest request)
    {
        return id != request.PaperId
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    //Write controller for DeleteQuestionInPaperRequest
    [HttpDelete("{id:guid}/questions/{questionId:guid}")]
    [OpenApiOperation("Delete a question in a paper")]
    [MustHavePermission(FSHAction.Delete, FSHResource.Papers)]
    public async Task<ActionResult> DeleteQuestionInPaperAsync(Guid id, Guid questionId)
    {
        return Ok(await Mediator.Send(new DeleteQuestionInPaperRequest { PaperId = id, QuestionCloneId = questionId }));
    }

    //fix controller for AddQuestionInPaperRequest

    [HttpPost("{id:guid}/questions")]
    [OpenApiOperation("Add questions in a paper")]
    [MustHavePermission(FSHAction.Update, FSHResource.Papers)]
    public async Task<ActionResult> UpdateQuestionInPaperAsync(Guid id,AddQuestionsInPaperRequest request)
    {
        if (id != request.PaperId)
        {
            return BadRequest("Paper Id in the request does not match the Id in the route.");
        }
        return Ok(await Mediator.Send(request));
    }

    [HttpGet("generate/docx")]
    public async Task<IActionResult> GeneratePaperDocx(Guid paperId)
    {
        var request = new GeneratePaperDocxRequest(paperId);
        var fileBytes = await Mediator.Send(request);

        // Trả về file DOCX dưới dạng response
       // return File(fileBytes, "application/msword");
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"ExamPaper-{paperId}.docx", true); //Download file
    }

    [HttpGet("generate/pdf")]
    public async Task<IActionResult> GeneratePaperPdf(Guid paperId)
    {
        var request = new GeneratePaperPdfRequest(paperId);
        var fileBytes = await Mediator.Send(request);

        //return File(fileBytes, "application/pdf");  //Get file in pdf format
        return File(fileBytes, "application/pdf", $"ExamPaper-{paperId}.pdf", true); //Download file
    }

}
