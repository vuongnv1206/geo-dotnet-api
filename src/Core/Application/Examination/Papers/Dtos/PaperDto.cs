using FSH.WebApi.Domain.Examination.Enums;
using FSH.WebApi.Application.Examination.PaperLabels;
using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Application.Subjects;


namespace FSH.WebApi.Application.Examination.Papers;
public class PaperDto : IDto
{
    public Guid Id { get; set; }
    public string ExamName { get; set; }
    public int NumberOfQuestion { get; set; }
    public int TotalAttended { get; set; }
    public int? Duration { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool ShowMarkResult { get; set; }
    public bool ShowQuestionAnswer { get; set; }
    public string? Password { get; set; }
    public string Type { get; set; }
   
    public bool IsPublish { get; set; }
    public string ExamCode { get; set; }
    public string? Content { get; set; }
    public string? Description { get; set; }
    public Guid? PaperFolderId { get; set; }
    public Guid? PaperLabelId { get; set; }
    public Guid? SubjectId { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatorName { get; set; }
    public DateTime CreatedOn { get; private set; }
    public Guid LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public SubjectDto Subject { get; set; }
    public float? MaxPoint => Questions.Sum(x => x.Mark);
    public PaperLabelDto PaperLable { get; set; }
    public PaperFolderDto PaperFolder { get; set; }
    public List<QuestionDto> Questions { get; set; }
}