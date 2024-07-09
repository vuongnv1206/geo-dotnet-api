using FSH.WebApi.Application.Examination.PaperStudents;

namespace FSH.WebApi.Host.Controllers.Examination;
public class PaperStudentsController : VersionedApiController
{
    [HttpPost("search")]
    [OpenApiOperation("get list of paper that student need to do")]
    public async Task<PaginationResponse<StudentTestDto>> GetPendingTests(GetPendingTestOfStudentRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPost("history/search")]
    [OpenApiOperation("get list of paper that student did")]
    public async Task<PaginationResponse<StudentTestHistoryDto>> GetHistoryTests(GetHistoryTestOfStudentRequest request)
    {
        return await Mediator.Send(request);
    }
}
