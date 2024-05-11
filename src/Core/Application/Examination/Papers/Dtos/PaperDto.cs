using FSH.WebApi.Domain.Examination.Enums;
using MediatR.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.Papers;
public class PaperDto : IDto
{
    public Guid Id { get; set; }
    public string ExamName { get; set; }
    public string Status { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public Guid? PaperLabelId { get; set; }
    public string? PaperLabelName { get; set; }
    public int NumberOfQuestion { get; set; }
    public int? Duration { get; set; }
    public bool Shuffle { get; set; }
    public bool ShowMarhResult { get; set; }
    public bool ShowQUestionAnswer { get; set; }
    public string? Password { get; set; }
    public string Type { get; set; }
    public Guid? FolderId { get; set; }
    public bool IsPublish { get; set; }
    public string ExamCode { get; set; }
    public string? Content { get; set; }
    public string? Description { get; set; }
}
