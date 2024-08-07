using FSH.WebApi.Application.Examination.PaperStatistics;
using FSH.WebApi.Application.Examination.PaperStatistics.Dtos;

namespace FSH.WebApi.Host.Controllers.Examination;

public class PaperStatisticsController : VersionedApiController
{

    // write controller for GetClassroomFrequencyMarkRequest
    [HttpGet("classroom-frequency-mark")]
    public async Task<ActionResult<List<ClassroomFrequencyMarkDto>>> GetClassroomFrequencyMark([FromQuery] GetClassroomFrequencyMarkRequest request)
    {
        return await Mediator.Send(request);
    }

    // write controller for GetListTranscriptRequest
    [HttpPost("list-transcript")]
    public async Task<ActionResult<TranscriptPaginationResponse>> GetListTranscript(GetListTranscriptRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpGet("get-exam-info/{paperId:Guid}")]
    [OpenApiOperation("get basic statistic info paper exam")]
    [MustHavePermission(FSHAction.View, FSHResource.Papers)]
    public Task<PaperInfoStatistic> GetInfoPaperStatistic(Guid paperId, Guid? classId)
    {
        return Mediator.Send(new GetPaperInfoRequest(paperId, classId));
    }

    [HttpPost("list-question-statistic")]
    [OpenApiOperation("Quesion statistic of a paper")]
    [MustHavePermission(FSHAction.View, FSHResource.Papers)]
    public Task<PaginationResponse<QuestionStatisticDto>> GetQuestionStatistic(GetQuestionStatisticRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("generate-excel")]
    public async Task<IActionResult> GenerateExcel([FromQuery] GeneratePaperStatisticExcelRequest request)
    {
        var fileBytes = await Mediator.Send(request);
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PaperStatistics.xlsx");
    }

}
