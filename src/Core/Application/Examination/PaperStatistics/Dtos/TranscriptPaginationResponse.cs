using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.PaperStatistics.Dtos;
public class TranscriptPaginationResponse : PaginationResponse<TranscriptResultDto>
{
    public TranscriptPaginationResponse(List<TranscriptResultDto> data, int count, int page, int pageSize, float averageMark)
        : base(data, count, page, pageSize)
    {
        AverageMark = averageMark;
    }
    public float AverageMark { get; set; }
}