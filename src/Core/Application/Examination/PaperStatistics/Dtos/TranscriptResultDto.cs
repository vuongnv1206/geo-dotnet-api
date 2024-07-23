

using FSH.WebApi.Application.Class.Dto;

namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class TranscriptResultDto
{
    public StudentDto Attendee { get; set; }
    public ClassDto Classroom { get; set; }
    public float Mark { get; set; }
    public DateTime StartedTest { get; set; }
    public DateTime? FinishedTest { get; set; }
}
