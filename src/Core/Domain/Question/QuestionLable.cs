namespace FSH.WebApi.Domain.Question;
public class QuestionLable : AuditableEntity, IAggregateRoot
{
    public string Name { get; set; } = null!;
    public string Color { get; set; } = "Primary";
}
