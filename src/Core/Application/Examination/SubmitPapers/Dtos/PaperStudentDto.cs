using FSH.WebApi.Application.Examination.PaperLabels;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class PaperStudentDto : IDto
{
    public Guid Id { get; set; }
    public string ExamName { get; set; }
    public Guid? PaperLabelId { get; set; }
    public int NumberOfQuestion { get; set; }
    public int? Duration { get; set; }
    public bool ShowMarkResult { get; set; }
    public bool ShowQuestionAnswer { get; set; }
    public string Type { get; set; }
    public bool IsPublish { get; set; }
    public string ExamCode { get; set; } = null!;
    public string? Description { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatorName { get; set; }
    public DateTime CreatedOn { get; private set; }
    public PaperLabelDto PaperLable { get; set; }
}
