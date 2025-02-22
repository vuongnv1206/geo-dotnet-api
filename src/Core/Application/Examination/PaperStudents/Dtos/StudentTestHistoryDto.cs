﻿using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperStudents.Dtos;
public class StudentTestHistoryDto : IDto
{
    public DefaultIdType Id { get; set; }
    public required string ExamName { get; set; }
    public Guid PaperId { get; set; }
    public float? Duration { get; set; }
    public DefaultIdType? PaperLabelId { get; set; }
    public string? PaperLabelName { get; set; }
    public DefaultIdType? SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public DateTime? StartedTime { get; set; }
    public DateTime? SubmittedTime { get; set; }
    public float? Score { get; set; }
    public CompletionStatusEnum CompletionStatus { get; set; }
    public ShowQuestionAnswer ShowQuestionAnswer { get; set; }
    public bool CanViewDetailAnswer { get; set; }
    public ShowResult ShowMarkResult { get; set; }
}
