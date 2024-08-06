// using DinkToPdf.Contracts;
// using DinkToPdf;
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
    // private readonly IPaperTemplateService _paperTemplateService;
    // private readonly IMediator _mediator;
    // private readonly IConverter _converter;

    // public GeneratePaperPdfRequestHandler(IPaperTemplateService paperTemplateService, IMediator mediator, IConverter converter)
    // {
    //    _paperTemplateService = paperTemplateService;
    //    _mediator = mediator;
    //    _converter = converter;
    // }

    // public async Task<byte[]> Handle(GeneratePaperPdfRequest request, CancellationToken cancellationToken)
    // {

    // var getPaperByIdRequest = new GetPaperByIdRequest(request.PaperId);
    //    var paperDto = await _mediator.Send(getPaperByIdRequest, cancellationToken);
    //    var paperTemplateModel = new PaperTemplateModel
    //    {
    //        ExamCode = paperDto.ExamCode,
    //        ExamName = paperDto.ExamName,
    //        StartTime = paperDto.StartTime,
    //        EndTime = paperDto.EndTime,
    //        Duration = paperDto.Duration,
    //        SubjectName = paperDto.Subject?.Name,
    //        Questions = paperDto.Questions.Adapt<List<QuestionModel>>(),
    //    };

    // var htmlContent = _paperTemplateService.GeneratePaperTemplate("paper",paperTemplateModel);

    // var globalSettings = new GlobalSettings
    //    {
    //        ColorMode = ColorMode.Color,
    //        Orientation = Orientation.Portrait,
    //        PaperSize = PaperKind.A4,
    //        Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 },
    //        DocumentTitle = "Generated PDF"
    //    };
    //    var objectSettings = new ObjectSettings
    //    {
    //        PagesCount = true,
    //        HtmlContent = htmlContent,
    //        WebSettings = { DefaultEncoding = "utf-8" },
    //        HeaderSettings = { FontSize = 12, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 },
    //        FooterSettings = { FontSize = 12, Line = true, Right = "© " + DateTime.Now.Year }
    //    };
    //    var pdf = new HtmlToPdfDocument()
    //    {
    //        GlobalSettings = globalSettings,
    //        Objects = { objectSettings }
    //    };
    //    var file = _converter.Convert(pdf);

    // return file;

    // }
    public Task<byte[]> Handle(GeneratePaperPdfRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

