
using FSH.WebApi.Application.Examination.Services;
using FSH.WebApi.Application.Examination.Services.Models;
using Mapster;

namespace FSH.WebApi.Application.Examination.Papers;
public class GeneratePaperPdfRequest : IRequest<byte[]>
{
    public Guid PaperId { get; set; }
    public GeneratePaperPdfRequest(Guid paperId)
    {
        PaperId = paperId;
    }
}

public class GeneratePaperPdfRequestHandler : IRequestHandler<GeneratePaperPdfRequest, byte[]>
{
    private readonly IPaperTemplateService _paperTemplateService;
    private readonly IMediator _mediator;
 

    public GeneratePaperPdfRequestHandler(IPaperTemplateService paperTemplateService, IMediator mediator)
    {
        _paperTemplateService = paperTemplateService;
        _mediator = mediator;
    }

    public async Task<byte[]> Handle(GeneratePaperPdfRequest request, CancellationToken cancellationToken)
    {

        var getPaperByIdRequest = new GetPaperByIdRequest(request.PaperId);
        var paperDto = await _mediator.Send(getPaperByIdRequest, cancellationToken);
        var paperTemplateModel = new PaperTemplateModel
        {
            ExamCode = paperDto.ExamCode,
            ExamName = paperDto.ExamName,
            StartTime = paperDto.StartTime,
            EndTime = paperDto.EndTime,
            Duration = paperDto.Duration.ToString(),
            SubjectName = paperDto.Subject?.Name,
            Questions = paperDto.Questions.Adapt<List<QuestionModel>>(),
        };

        var htmlContent = _paperTemplateService.GeneratePaperTemplate("paper", paperTemplateModel);

        var file = await _paperTemplateService.GeneratePdfFromHtml(htmlContent, paperTemplateModel.ExamName);
       
        return file;

    }
}

