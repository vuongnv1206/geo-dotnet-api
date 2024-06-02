using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Examination.PaperLabels;
using FSH.WebApi.Domain.Examination.Enums;


namespace FSH.WebApi.Application.Examination.Papers;
public class PaperInListDto : IDto
{
    public Guid Id { get; set; }
    public string ExamName { get; set; }
    public Guid? PaperLabelId { get; set; }
    public int NumberOfQuestion { get; set; }
    public int? Duration { get; set; }
    public bool ShowMarkResult { get; set; }
    public bool ShowQuestionAnswer { get; set; }
    public string? Password { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }
    public Guid? PaperFolderId { get; set; }
    public bool IsPublish { get; set; }
    public string ExamCode { get; set; }
    public string? Content { get; set; }
    public string? Description { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; private set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public PaperLabelDto PaperLabel { get; set; }
    public PaperFolderDto PaperFolder { get; set; }
    public List<PaperFolderParentDto>? Parents { get; set; }

}