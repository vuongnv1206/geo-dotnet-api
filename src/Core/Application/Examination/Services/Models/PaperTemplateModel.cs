using FSH.WebApi.Application.Questions.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.Services.Models;
public class PaperTemplateModel
{
    //public string FullName { get; set; }
    //public string StudentCode { get; set; }
    //public string ClassName { get; set; }
    public string SubjectName { get; set; }
    public string ExamName { get; set; }
    public string ExamCode { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Duration { get; set; }
    public int TotalQuestion { get; set; }
    public List<QuestionModel> Questions { get; set; }

}
