using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Examination.PaperLabels;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Application.Subjects;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Examination.Enums;

namespace FSH.WebApi.Application.Examination.Papers.Dtos;
public class PaperDto : IDto
{
    public DefaultIdType Id { get; set; }
    public required string ExamName { get; set; }
    public int NumberOfQuestion { get; set; }
    public int TotalAttended { get; set; }
    public float? Duration { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public ShowResult ShowMarkResult { get; set; }
    public ShowQuestionAnswer ShowQuestionAnswer { get; set; }
    public string? Password { get; set; }
    public required string Type { get; set; }
    public PaperShareType ShareType { get; set; }
    public bool IsPublish { get; set; }
    public PaperStatus Status { get; set; }
    public required string ExamCode { get; set; }
    public string? Content { get; set; }
    public string? Description { get; set; }
    public DefaultIdType? PaperFolderId { get; set; }
    public DefaultIdType? PaperLabelId { get; set; }
    public DefaultIdType? SubjectId { get; set; }
    public DefaultIdType CreatedBy { get; set; }
    public required string CreatorName { get; set; }
    public DateTime CreatedOn { get; private set; }
    public DefaultIdType LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public required SubjectDto Subject { get; set; }
    public float? MaxPoint => Questions.Sum(x => x.Mark);
    public string? PublicIpAllowed { get; set; }
    public string? LocalIpAllowed { get; set; }
    public required PaperLabelDto PaperLable { get; set; }
    public required PaperFolderDto PaperFolder { get; set; }
    public required List<QuestionDto> Questions { get; set; }
    public required List<PaperAccessDto> PaperAccesses { get; set; }
    public required List<PaperPermissionDto> PaperPermissions { get; set; }
}

public class PaperMoniDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? ExamName { get; set; }
    public int NumberOfQuestion { get; set; }
    public int TotalAttended { get; set; }
    public float? Duration { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public ShowResult ShowMarkResult { get; set; }
    public ShowQuestionAnswer ShowQuestionAnswer { get; set; }
    public string? Password { get; set; }
    public string? Type { get; set; }
    public string? ExamCode { get; set; }
    public string? Content { get; set; }
    public string? Description { get; set; }
    public DefaultIdType CreatedBy { get; set; }
    public string? CreatorName { get; set; }
    public DateTime CreatedOn { get; private set; }
    public DefaultIdType LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string? PublicIpAllowed { get; set; }
    public string? LocalIpAllowed { get; set; }
    public float? MaxPoint { get; set; }
}