namespace FSH.WebApi.Application.Questions.Dtos;

public class QuestionLableDto : IDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string Color { get; set; } = "Primary";
}