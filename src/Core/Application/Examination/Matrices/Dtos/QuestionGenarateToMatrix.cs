using FSH.WebApi.Application.Questions.Dtos;

namespace FSH.WebApi.Application.Examination.Matrices;
public class QuestionGenarateToMatrix : IDto
{
    public QuestionDto Question { get; set; }
    public float Mark { get; set; }
    public int? RawIndex { get; set; }
}
