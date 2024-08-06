using FSH.WebApi.Application.Common.Exporters;
using FSH.WebApi.Application.Examination.PaperStatistics.Dtos;
using FSH.WebApi.Application.Examination.Services;


namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class GeneratePaperStatisticExcelRequest : IRequest<byte[]>
{
    public Guid PaperId { get; set; }
    public List<RequestStatisticType> RequestStatisticTypes { get; set; }
}

public class GeneratePaperStatisticExcelRequestHandler : IRequestHandler<GeneratePaperStatisticExcelRequest, byte[]>
{
    private readonly IMediator _mediator;
    private readonly IPaperTemplateService _paperTemplateService;

    public GeneratePaperStatisticExcelRequestHandler(IMediator mediator, IPaperTemplateService paperTemplateService)
    {
        _mediator = mediator;
        _paperTemplateService = paperTemplateService;
    }

    public async Task<byte[]> Handle(GeneratePaperStatisticExcelRequest request, CancellationToken cancellationToken)
    {
        byte[] result = null;

        foreach (var requestType in request.RequestStatisticTypes)
        {
            switch (requestType)
            {
                case RequestStatisticType.GetClassroomFrequencyMark:
                    var frequencyMarkData = await _mediator.Send(new GetClassroomFrequencyMarkRequest { PaperId = request.PaperId }, cancellationToken);
                    result = _paperTemplateService.GenerateFrequencyMarkSheet(frequencyMarkData);
                    break;

                case RequestStatisticType.GetListTranscript:
                    var transcriptData = await _mediator.Send(new GetListTranscriptRequest { PaperId = request.PaperId }, cancellationToken);
                    result = _paperTemplateService.GenerateTranscriptSheet(transcriptData);
                    break;

                case RequestStatisticType.GetPaperInfor:
                    var questionStatisticData = await _mediator.Send(new GetPaperInfoRequest(request.PaperId, null), cancellationToken);
                    result = _paperTemplateService.GeneratePaperInfoSheet(questionStatisticData);
                    break;


                default:
                    throw new ArgumentException("Invalid request type");
            }
        }

        return result;
    }
}

