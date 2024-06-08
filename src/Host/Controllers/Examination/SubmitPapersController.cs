using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.Papers.ByStudents;
using FSH.WebApi.Application.Examination.Reviews;
using FSH.WebApi.Application.Examination.SubmitPapers;

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

    [HttpGet("paper/{paperId:guid}")]
    [OpenApiOperation("get information of paper by role student")]
    public async Task<PaperStudentDto> GetPapperByRoleStudent(Guid paperId)
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


    [HttpGet("last-result")]
    public async Task<ActionResult<LastResultExamDto>> GetLastResult([FromQuery] GetLastResultExamRequest request)
    {
        var result = await Mediator.Send(request);
        return Ok(result);
    }

}
