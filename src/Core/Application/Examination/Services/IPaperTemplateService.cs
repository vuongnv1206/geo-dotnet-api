using FSH.WebApi.Application.Examination.PaperStatistics.Dtos;
using FSH.WebApi.Application.Examination.PaperStatistics;


namespace FSH.WebApi.Application.Examination.Services;
public interface IPaperTemplateService : ITransientService
{
    string GeneratePaperTemplate<T>(string templateName, T paperTemplateModel);
    byte[] GeneratePaperStatisticExcel(
        List<ClassroomFrequencyMarkDto> frequencyMarkData,
        TranscriptPaginationResponse transcriptData,
        PaperInfoStatistic paperInfoData
    );
    Task<byte[]> GeneratePdfFromHtml(string htmlContent, string title);


}
