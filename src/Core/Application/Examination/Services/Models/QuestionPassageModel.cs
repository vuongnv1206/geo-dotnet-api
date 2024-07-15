using FSH.WebApi.Application.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.Services.Models;
public class QuestionPassageModel
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public List<AnswerModel> Answers { get; set; }
}
