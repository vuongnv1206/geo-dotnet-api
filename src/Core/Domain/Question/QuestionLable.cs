namespace FSH.WebApi.Domain.Question;
public class QuestionLable : AuditableEntity, IAggregateRoot
{
    public string Name { get; set; } = null!;
    public string Color { get; set; } = "Primary";

    public QuestionLable()
    {

    }

    public QuestionLable(string name, string color)
    {
        Name = name;
        Color = color;
    }

    public void Update(string name, string color)
    {
        Name = name;
        Color = color;
    }
}

