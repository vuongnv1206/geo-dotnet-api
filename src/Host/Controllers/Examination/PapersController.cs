
using FSH.WebApi.Application.Examination.Papers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FSH.WebApi.Host.Controllers.Examination;
public class PapersController : VersionedApiController
{
    [HttpPost("Search")]
    [OpenApiOperation("")]
    public Task<PaginationResponse<PaperDto>> SearchPaperLabel(SearchPaperRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [OpenApiOperation("Get paper details.", "")]
    public Task<PaperDto> GetAsync(Guid id)
    {
        return Mediator.Send(new GetPaperByIdRequest(id));
    }
}
