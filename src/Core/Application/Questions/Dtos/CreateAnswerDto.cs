namespace FSH.WebApi.Application.Questions;
public class CreateAnswerDto : IDto
{
    public string? Content { get; set; }
    public bool IsCorrect { get; set; }
}