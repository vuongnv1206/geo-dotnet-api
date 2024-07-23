using FSH.WebApi.Application.Examination.PaperStatistics;

namespace FSH.WebApi.Host.Controllers.Examination;

public class PaperStatisticsController : VersionedApiController
{
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
}
