using FSH.WebApi.Application.Examination.PaperStatistics;
using FSH.WebApi.Application.Examination.PaperStatistics.Dtos;

namespace FSH.WebApi.Host.Controllers.Examination;

public class PaperStatisticsController : VersionedApiController
{

    //write controller for GetClassroomFrequencyMarkRequest
    [HttpGet("classroom-frequency-mark")]
    public async Task<ActionResult<ClassroomFrequencyMarkDto>> GetClassroomFrequencyMark([FromQuery] GetClassroomFrequencyMarkRequest request)
    {
        return await Mediator.Send(request);
    }

    //write controller for GetListTranscriptRequest
    [HttpPost("list-transcript")]
    public async Task<ActionResult<TranscriptPaginationResponse>> GetListTranscript(GetListTranscriptRequest request)
    {
        return await Mediator.Send(request);
    }
}
