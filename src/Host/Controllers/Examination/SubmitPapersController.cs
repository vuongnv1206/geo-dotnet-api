﻿using FSH.WebApi.Application.Examination.SubmitPapers;

namespace FSH.WebApi.Host.Controllers.Examination;
public class SubmitPapersController : VersionedApiController
{
    [HttpPost]
    [OpenApiOperation("Create a submit paper - when user start do exam")]
    public async Task<Guid> CreateSubmitPaperAsync(CreateSubmitPaperRequest request)
    {
        return await Mediator.Send(request);
    }

}
