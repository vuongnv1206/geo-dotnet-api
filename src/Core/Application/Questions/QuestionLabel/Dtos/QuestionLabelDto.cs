namespace FSH.WebApi.Application.Questions.QuestionLabel.Dtos;
public class QuestionLabelDto : IDto
{
    public DefaultIdType? Id { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }
}
