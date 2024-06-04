﻿using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.Papers.ByStudents;
using FSH.WebApi.Application.Examination.SubmitPapers;

namespace FSH.WebApi.Host.Controllers.Examination;
public class SubmitPapersController : VersionedApiController
{
    [HttpPost]
    [OpenApiOperation("Create a submit paper - when user start do exam")]
    public async Task<Guid> CreateSubmitPaperAsync(CreateSubmitPaperRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPost("submit-answer")]
    [OpenApiOperation("Create a submit paper detail - when user choose answer for question")]
    public async Task<Guid> SubmitAnswerForQuestion(SubmitAnswerRawRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpGet("paper/{id:guid}")]
    [OpenApiOperation("get information of paper by role student")]
    public async Task<PaperStudentDto> GetPapperByRoleStudent(Guid id)
    {
        return await Mediator.Send(new GetPaperByIdRoleStudentRequest(id));
    }

}
