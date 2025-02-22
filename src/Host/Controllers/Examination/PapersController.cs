﻿using FSH.WebApi.Application.Examination.Matrices;
using FSH.WebApi.Application.Examination.PaperAccesses;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.Papers.Dtos;

namespace FSH.WebApi.Host.Controllers.Examination;
public class PapersController : VersionedApiController
{
    [HttpPost("Search")]
    [OpenApiOperation("Search paper using available filter", "")]
    [MustHavePermission(FSHAction.View, FSHResource.Papers)]
    public Task<List<PaperInListDto>> SearchPaper(SearchPaperRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost("search-deleted")]
    [OpenApiOperation("Search deleted papers", "")]
    [MustHavePermission(FSHAction.View, FSHResource.Papers)]
    public Task<PaginationResponse<PaperDeletedDto>> SearchDeletedPaper(SearchPaperDeletedRequest request)
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

    [HttpPut("restore-deleted")]
    [OpenApiOperation("Restore a deleted paper")]
    public async Task<List<Guid>> RestoreAsync(RestoreDeletedPapersRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpDelete]
    [OpenApiOperation("Delete multiple papers")]
    [MustHavePermission(FSHAction.Delete, FSHResource.Papers)]
    public async Task<List<Guid>> DeleteMultipleAsync(DeletePapersRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPut("{id:guid}/questions")]
    [OpenApiOperation("Update questions in a paper")]
    [MustHavePermission(FSHAction.Update, FSHResource.Papers)]
    public async Task<ActionResult> UpdateQuestionsInPaperAsync(Guid id, UpdateQuestionsInPaperRequest request)
    {
        if (id != request.PaperId)
        {
            return BadRequest("Paper Id in the request does not match the Id in the route.");
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

    // Write controller for DeleteQuestionInPaperRequest
    [HttpDelete("{id:guid}/questions/{questionId:guid}")]
    [OpenApiOperation("Delete a question in a paper")]
    [MustHavePermission(FSHAction.Delete, FSHResource.Papers)]
    public async Task<ActionResult> DeleteQuestionInPaperAsync(Guid id, Guid questionId)
    {
        return Ok(await Mediator.Send(new DeleteQuestionInPaperRequest { PaperId = id, OriginalQuestionId = questionId }));
    }

    // fix controller for AddQuestionInPaperRequest

    [HttpPost("{id:guid}/questions")]
    [OpenApiOperation("Add questions in a paper")]
    [MustHavePermission(FSHAction.Update, FSHResource.Papers)]
    public async Task<ActionResult> UpdateQuestionInPaperAsync(Guid id, AddQuestionsInPaperRequest request)
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
        byte[] fileBytes = await Mediator.Send(request);

        // Trả về file DOCX dưới dạng response
        // return File(fileBytes, "application/msword");
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"ExamPaper-{paperId}.docx", true); // Download file
    }

    [HttpGet("generate/pdf")]
    public async Task<IActionResult> GeneratePaperPdf(Guid paperId)
    {
        var request = new GeneratePaperPdfRequest(paperId);
        var fileBytes = await Mediator.Send(request);

        //return File(fileBytes, "application/pdf");  //Get file in pdf format
        return File(fileBytes, "application/pdf", $"ExamPaper-{paperId}.pdf", true); //Download file
    }

    // Write controller for CreatePaperFromMatrixRequest
    [HttpPost("get-questions-from-matrix")]
    [OpenApiOperation("Get generated questions from matrix")]
    [MustHavePermission(FSHAction.Create, FSHResource.Papers)]
    public async Task<ActionResult<List<QuestionGenerateToMatrix>>> CreateFromMatrixAsync(CreatePaperFromMatrixRequest request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpPost("get-access-paper")]
    [OpenApiOperation("get group class and student who assigned to paper")]
    [MustHavePermission(FSHAction.View, FSHResource.Papers)]
    public async Task<PaginationResponse<GroupClassAccessPaper>> GetGroupClassesAccessPaper(GetGroupClassesAccessPaperRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPost("get-assignees-paper")]
    [OpenApiOperation("get student who assigned to paper")]
    [MustHavePermission(FSHAction.View, FSHResource.Papers)]
    public async Task<PaginationResponse<ClassAccessPaper>> GetAssigneesInPaper(GetGetAssigneesInPaperRequest request)
    {
        return await Mediator.Send(request);
    }
}
