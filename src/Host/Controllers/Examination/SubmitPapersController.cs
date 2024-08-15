using FSH.WebApi.Application.Examination.Monitor;
using FSH.WebApi.Application.Examination.Monitor.Dtos;
using FSH.WebApi.Application.Examination.Papers.ByStudents;
using FSH.WebApi.Application.Examination.Papers.Dtos;
using FSH.WebApi.Application.Examination.Reviews;
using FSH.WebApi.Application.Examination.SubmitPapers;
using FSH.WebApi.Application.Examination.SubmitPapers.Dtos;

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

    [HttpPut("{id}")]
    [OpenApiOperation("Update ending status submit paper")]
    public async Task<ActionResult<Guid>> EndingSubmitPaper(UpdateSubmitPaperRequest request, Guid id)
    {
        return id == request.Id
            ? Ok(await Mediator.Send(request))
            : BadRequest();
    }

    [HttpGet("paper/{paperId:guid}")]
    [OpenApiOperation("get information of paper by role student")]
    public async Task<PaperStudentDto> GetPaperByRoleStudent(Guid paperId)
    {
        return await Mediator.Send(new GetPaperByIdRoleStudentRequest(paperId));
    }

    [HttpPost("paper/{paperId:guid}/students-submitted")]
    [OpenApiOperation("get student have submitted a paper yet")]
    public async Task<ActionResult<List<SubmitPaperDto>>> GetSubmittedPaper(Guid paperId, GetSubmittedPaperRequest request)
    {
        return paperId == request.PaperId
            ? Ok(await Mediator.Send(request))
            : BadRequest();
    }

    [HttpPost("last-result")]
    public async Task<ActionResult<LastResultExamDto>> GetLastResult(GetLastResultExamRequest request)
    {
        var result = await Mediator.Send(request);
        return Ok(result);
    }

    [HttpPost("mark-answer")]
    [OpenApiOperation("Mark answer")]
    public async Task<Guid> MarkAnswer(MarkAnswerRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPost("start")]
    [OpenApiOperation("Start exam")]
    public async Task<ActionResult<PaperForStudentDto>> StartExam(StartExamRequest request)
    {
        request.PublicIp = GetIpAddress();
        return await Mediator.Send(request);
    }

    [HttpPost("submit")]
    [OpenApiOperation("Submit exam")]
    public async Task<ActionResult<Guid>> SubmitExam(SubmitExamRequest request)
    {
        request.PublicIp = GetIpAddress();
        return await Mediator.Send(request);
    }

    [HttpPost("send-log")]
    [OpenApiOperation("Send log")]
    public async Task<ActionResult<Guid>> SendLog(SendLogRequest request)
    {
        request.PublicIp = GetIpAddress();
        return await Mediator.Send(request);
    }

    [HttpPost("monitor")]
    [OpenApiOperation("Monitor exam")]
    public async Task<ActionResult<MonitorExamDto>> MonitorExam(MonitorExamRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPost("monitor-detail")]
    [OpenApiOperation("Monitor detail of user in exam")]
    public async Task<ActionResult<Guid>> MonitorDetailExam(MonitorDetailExamRequest request)
    {
        return await Mediator.Send(request);
    }

    [HttpPost("reassign")]
    [OpenApiOperation("Reassign exam")]
    public async Task<ActionResult<Guid>> ReassignExam(ReassignExamRequest request)
    {
        return await Mediator.Send(request);
    }

    public string? GetIpAddress() =>
    Request.Headers.ContainsKey("X-Forwarded-For")
        ? Request.Headers["X-Forwarded-For"]
        : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";

}
