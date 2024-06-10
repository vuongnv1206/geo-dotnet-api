

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class SubmitPaperDetailDto
{
    public Guid SubmitPaperId { get; set; }
    public Guid QuestionId { get; set; }
    public string? AnswerRaw { get; set; }
    public float? Mark { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}
