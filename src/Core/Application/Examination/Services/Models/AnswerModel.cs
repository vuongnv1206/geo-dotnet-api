

namespace FSH.WebApi.Application.Examination.Services.Models;
public class AnswerModel
{
    public Guid? Id { get; set; }
    public string? Content { get; set; }
    public Guid? QuestionId { get; set; }
    //public bool IsCorrect { get; set; }
}
