using FSH.WebApi.Application.Examination.Papers;

namespace FSH.WebApi.Host.Controllers.Examination;
public class PapersController : VersionedApiController
{
    [HttpPost]
    [OpenApiOperation("Create a paper.")]
    public Task<PaperDto> CreateAsync(CreatePaperRequest request)
    {
        return Mediator.Send(request);
    }
}
