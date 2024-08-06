using FSH.WebApi.Application.Examination.PaperStatistics.Dtos;
using FSH.WebApi.Application.Examination.PaperStatistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.Services;
public interface IPaperTemplateService : ITransientService
{
    string GeneratePaperTemplate<T>(string templateName, T paperTemplateModel);

    byte[] GenerateFrequencyMarkSheet(List<ClassroomFrequencyMarkDto> data);
    byte[] GenerateTranscriptSheet(TranscriptPaginationResponse data);
    byte[] GeneratePaperInfoSheet(PaperInfoStatistic data);

}
