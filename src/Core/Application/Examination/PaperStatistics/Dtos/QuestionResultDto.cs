
namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class QuestionResultDto
{
    public int RawIndex { get; set; }
    public string IsCorrect { get; set; }
    public float Score { get; set; }
    public List<QuestionResultDto> QuestionPassages { get; set; } = new List<QuestionResultDto>();
}
