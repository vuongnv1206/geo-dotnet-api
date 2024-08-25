using FSH.WebApi.Application.Common.Exporters;
using FSH.WebApi.Application.Examination.PaperStatistics.Dtos;
using FSH.WebApi.Application.Examination.Services;


namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class GeneratePaperStatisticExcelRequest : IRequest<byte[]>
{
    public Guid PaperId { get; set; }
    public Guid? ClassId { get; set; }
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
        List<ClassroomFrequencyMarkDto> frequencyMarkData = null;
        TranscriptPaginationResponse transcriptData = null;
        PaperInfoStatistic paperInfoData = null;

        foreach (var requestType in request.RequestStatisticTypes)
        {
            switch (requestType)
            {
                case RequestStatisticType.GetClassroomFrequencyMark:
                    frequencyMarkData = await _mediator.Send(new GetClassroomFrequencyMarkRequest { PaperId = request.PaperId , ClassroomId = request.ClassId }, cancellationToken);
                    break;

                case RequestStatisticType.GetListTranscript:
                    transcriptData = await _mediator.Send(new GetListTranscriptRequest { PaperId = request.PaperId, ClassId = request.ClassId }, cancellationToken);
                    break;

                case RequestStatisticType.GetPaperInfor:
                    paperInfoData = await _mediator.Send(new GetPaperInfoRequest(request.PaperId, request.ClassId), cancellationToken);
                    break;

                default:
                    throw new ArgumentException("Invalid request type");
            }
        }

        // Generate the Excel file with the collected data
        var result = _paperTemplateService.GeneratePaperStatisticExcel(frequencyMarkData, transcriptData, paperInfoData);
        return result;
    }
}

