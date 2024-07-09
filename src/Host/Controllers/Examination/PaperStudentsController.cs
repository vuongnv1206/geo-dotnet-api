using FSH.WebApi.Application.Examination.PaperStudents;
using FSH.WebApi.Application.Examination.PaperStudents.Dtos;

namespace FSH.WebApi.Host.Controllers.Examination;
public class PaperStudentsController : VersionedApiController
{
    [HttpPost("search")]
    [OpenApiOperation("get list of paper that student need to do")]
    public async Task<PaginationResponse<StudentTestDto>> GetPendingTests(GetPendingTestOfStudentRequest request)
    {
        return await Mediator.Send(request);
    }
}
