using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Examination.PaperLabels;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Application.Subjects;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Papers.Dtos;

public class PaperForStudentDto
{
    public Guid Id { get; set; }
    public required string ExamName { get; set; }
    public int NumberOfQuestion { get; set; }
    public int TotalAttended { get; set; }
    public int? Duration { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public ShowResult ShowMarkResult { get; set; }
    public ShowQuestionAnswer ShowQuestionAnswer { get; set; }
    public string? Password { get; set; }
    public required string Type { get; set; }
    public bool IsPublish { get; set; }
    public required string ExamCode { get; set; }
    public string? Content { get; set; }
    public string? Description { get; set; }
    public Guid? PaperFolderId { get; set; }
    public Guid? PaperLabelId { get; set; }
    public Guid? SubjectId { get; set; }
    public Guid CreatedBy { get; set; }
    public required string CreatorName { get; set; }
    public DateTime CreatedOn { get; private set; }
    public Guid LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public required SubjectDto Subject { get; set; }
    public float? MaxPoint => Questions.Sum(x => x.Mark);
    public required PaperLabelDto PaperLable { get; set; }
    public required PaperFolderDto PaperFolder { get; set; }
    public required List<QuestionForStudentDto> Questions { get; set; }
    public Guid SubmitPaperId { get; set; }
    public required UserDetailsDto UserDetails { get; set; }
}