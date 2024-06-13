namespace FSH.WebApi.Application.Questions;
public class NewQuestionDto : CreateQuestionDto
{
    public float Mark { get; set; }
    public int? RawIndex { get; set; }
}