using FSH.WebApi.Application.Examination.PaperStatistics;

namespace FSH.WebApi.Host.Controllers.Examination;

public class PaperStatisticsController : VersionedApiController
{

    //write controller for GetClassroomFrequencyMarkRequest
    [HttpGet("classroom-frequency-mark")]
    public async Task<ActionResult<ClassroomFrequencyMarkDto>> GetClassroomFrequencyMark([FromQuery] GetClassroomFrequencyMarkRequest request)
    {
        return await Mediator.Send(request);
    }
}
